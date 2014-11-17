using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using aulease.Entities;
using System.Diagnostics;

namespace LeasingDatabase.Core
{
	public static class Scan
	{
		public static string[] Parse(string file, ref List<string> errors)
		{
			string[] words = file.Split(' ', '\t', '\n');

			string[] Array = new string[words.Count()];
			HashSet<string> SR = new HashSet<string>();

			int e = 0;

			foreach (string word in words)
			{
				Array[e] = word.Trim().TrimEnd('\r', '\n').ToUpper();
				e++;
			}

			int num1;
			int LastIdx = -1;
			int length = Array.Length;

			for (int i = 0; i < length; i++)
			{
				AuleaseEntities db = new AuleaseEntities();
				List<Component> components = db.Components.Where(n => n.SerialNumber == null || n.SerialNumber.Length < 2).ToList();
				bool res;
				try
				{ res = int.TryParse((Array[i].Substring(2, 5)), out num1); }
				catch
				{ res = false; }

				bool res2;
				try
				{ res2 = Array[i].StartsWith("AU"); }
				catch
				{ res2 = false; }

				bool res3;
				bool res4;
				if (res == true & res2 == true)
				{
					if ((i + 2) < length)
					{
						res3 = Array[i + 2].StartsWith("AU");
						try
						{ res4 = Int32.TryParse(Array[i + 2].Substring(2, 5), out num1); }
						catch
						{ res4 = false; }
					}
					else
					{
						res3 = false;
						res4 = false;
					}


					if (res3 == true & res4 == true)
					{
						continue;
					}
					else
					{
						if (i - LastIdx == 3)
						{
							List<Component> Order = components.Where(n => n.OrderNumber == Array[i - 2]).ToList();
                            if (Order.Count == 0) { LastIdx = i; db.SaveChanges(); errors.Add("No order number found"); continue; }
							SystemGroup SystemGroup = Order.First().SystemGroup;

							if (SystemGroup.ComponentCount == 1)
							{
                                string Serial1 = Array[i - 1];
                                if (db.Components.Any(n => n.SerialNumber.Contains(Serial1)))
                                {
                                    LastIdx = i;
                                    db.SaveChanges();
                                    errors.Add("Serial Number already exists");
                                    continue;
                                }

								Component comp = SystemGroup.ActiveLeases.First().Component;
								comp.SerialNumber = Array[i - 1];
								comp.LeaseTag = Array[i];

								SR.Add(comp.PO.PONumber);
							}
							LastIdx = i;
						}

						else if (i - LastIdx == 5)
						{
							List<Component> Order = components.Where(n => n.OrderNumber == Array[i - 4]).ToList();
                            if (Order.Count == 0) { LastIdx = i; db.SaveChanges(); errors.Add("No order number found"); continue; }
							SystemGroup SystemGroup = Order.First().SystemGroup;

							if (SystemGroup.ComponentCount == 2)
							{
                                string Serial1 = Array[i - 3];
                                string Serial2 = Array[i - 1];

                                if (db.Components.Any(n => n.SerialNumber.Contains(Serial1) || n.SerialNumber.Contains(Serial2)))
                                {
                                    LastIdx = i;
                                    db.SaveChanges();
                                    errors.Add("Serial Number already exists");
                                    continue;
                                }

                                if (!ContainsTwoMonitors(SystemGroup))
                                {
                                    Component comp = SystemGroup.Components.Where(n => n.Type.Name != "Monitor").Single();
                                    comp.SerialNumber = Array[i - 3];
                                    comp.LeaseTag = Array[i - 2];
                                    Component mon = SystemGroup.Components.Where(n => n.Type.Name == "Monitor").First();
                                    mon.SerialNumber = Array[i - 1];
                                    mon.LeaseTag = Array[i];
                                }
                                else
                                {
                                    List<Component> mon = SystemGroup.Components.Where(n => n.Type.Name == "Monitor").ToList();
                                    mon[0].SerialNumber = Array[i - 3];
                                    mon[0].LeaseTag = Array[i - 2];
                                    mon[1].SerialNumber = Array[i - 1];
                                    mon[1].LeaseTag = Array[i];
                                }

								SR.Add(SystemGroup.PO.PONumber);
							}
							LastIdx = i;
						}

						else if (i - LastIdx == 7)
						{
							List<Component> Order = components.Where(n => n.OrderNumber == Array[i - 6]).ToList();
                            if (Order.Count == 0) { LastIdx = i; db.SaveChanges(); errors.Add("No order number found"); continue; }
							SystemGroup SystemGroup = Order.First().SystemGroup;

							if (SystemGroup.ComponentCount == 3)
							{
                                string Serial1 = Array[i - 5];
                                string Serial2 = Array[i - 3];
                                string Serial3 = Array[i - 1];

                                if (db.Components.Any(n => n.SerialNumber.Contains(Serial1) || n.SerialNumber.Contains(Serial2) || n.SerialNumber.Contains(Serial3)))
                                {
                                    LastIdx = i;
                                    db.SaveChanges();
                                    errors.Add("Serial Number already exists");
                                    continue;
                                }

								Component comp = SystemGroup.Components.Where(n => n.Type.Name != "Monitor").Single();
								comp.SerialNumber = Array[i - 5];
								comp.LeaseTag = Array[i-4];
								List<Component> mon = SystemGroup.Components.Where(n => n.Type.Name == "Monitor").ToList();
								mon[0].SerialNumber = Array[i - 3];
								mon[0].LeaseTag = Array[i-2];
								mon[1].SerialNumber = Array[i - 1];
								mon[1].LeaseTag = Array[i];

                                

								SR.Add(comp.PO.PONumber);
							}
							LastIdx = i;
						}
					}
				}

				else
				{
				}

				db.SaveChanges();
			}

			

			string[] SRs = new string[SR.Count];
			int count = 0;

			foreach (string item in SR)
			{
				SRs[count] = item;
				count++;
			}
			return SRs;

		}

        private static bool ContainsTwoMonitors(SystemGroup SystemGroup)
        {
            return SystemGroup.Components.Where(n => n.Type.Name == "Monitor").Count() > 1 ? true : false;
        }
	}
}