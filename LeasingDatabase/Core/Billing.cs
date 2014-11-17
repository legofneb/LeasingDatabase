using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Core
{
	public class Billing
	{
        static List<string> ChargedComponent = new List<string>()
        {
            "CPU",
            "LAPTOP",
            "SERVER",
            "MONITOR"
        };

		public static decimal CalculateMonthlyCharge(int LeaseID)
		{
			AuleaseEntities db = new AuleaseEntities();
			Lease lease = db.Leases.Where(n => n.Id == LeaseID).Single();
			decimal MonthlyCharge = 0.00M;
            
			List<Charge> charges = lease.Charges.ToList();

			foreach (var charge in charges)
			{
                decimal term = lease.Overhead.Term;
				if (ChargedComponent.Contains(charge.Type.Name.ToUpper()))
				{
					decimal OverheadRate = lease.Overhead.Rate;
					decimal tax = charge.Tax.Price;
					decimal VendorRate = charge.VendorRate.Rate;
					MonthlyCharge =+ MonthlyCharge + ((((charge.Price * VendorRate / 1000) * (1 + tax)) + (OverheadRate)) * ((term+1) / term));
				}
				else
				{
					MonthlyCharge =+ MonthlyCharge + ((charge.VendorRate.Rate * charge.Price / 1000) * ((term+1) / term));
				}
			}
			return MonthlyCharge;
		}

        public static string CreateStringSummary(int SystemGroupId)
        {
            AuleaseEntities db = new AuleaseEntities();
            SystemGroup systemGroup = db.SystemGroups.Where(n => n.Id == SystemGroupId).Single();
            string summary = "";

            List<Lease> leases = systemGroup.Leases.OrderBy(n => n.Component.TypeId).ToList();
            string lineBreak = "\n";

            int i = 1;
            foreach (var lease in leases)
            {
                summary += "Component " + i.ToString() + ": " + lease.Component.Type.Name + lineBreak;
                i++;
                decimal TotalCost = lease.Charges.Where(n => n.TypeId == lease.Component.TypeId).Single().Price;
                summary += "Total Cost: " + TotalCost.ToString() + lineBreak;
                summary += "Leasing Rate: " + CalculateLeasingRate(lease.ComponentId).ToString() + lineBreak;
                summary += "Insurance/Shipping/Warranty Rate: " + CalculateSecondaryCosts(lease.ComponentId).ToString("###.###") + lineBreak;
                summary += "Overhead Rate: " + lease.Overhead.Rate + lineBreak;
                summary += "Vendor Rate: " + lease.Charges.Where(n => n.TypeId == lease.Component.TypeId).Single().VendorRate.Rate + lineBreak;
                summary += "Vendor Insurance Rate: " + (lease.Charges.Where(n => n.Type.Name == "Insurance").Count() > 0 ? lease.Charges.Where(n => n.Type.Name == "Insurance").FirstOrDefault().VendorRate.Rate.ToString("###.###") : "NaN") + lineBreak;
                summary += "Tax: " + CalculateTax(lease.Component.Id).ToString("##.##") + lineBreak;
                summary += "Monthly Charge: " + CalculateMonthlyCharge(lease.Id).ToString("###.###") + lineBreak;
                summary += lineBreak;
            }

            return summary;
        }

		public static void FillLeasingInfo(int SystemGroupID, string BegBillDate, string EndBillDate,
											string CPUCost, string MonitorCost, string Monitor2Cost,
											string InsuranceCost, string WarrantyCost, string ShippingCost)
		{
			AuleaseEntities db = new AuleaseEntities();

			SystemGroup system = db.SystemGroups.Where(n => n.Id == SystemGroupID).Single();

            //Remove existing charges before adding new ones
            foreach (var lease in system.Leases)
            {
                List<Charge> charges = lease.Charges.ToList();
                foreach (var charge in charges)
                {
                    lease.Charges.Remove(charge);
                }

                foreach (var charge in db.Charges.Where(n => n.Leases.FirstOrDefault().Id == lease.Id))
                {
                    db.Entry(charge).State = EntityState.Deleted;
                }
            }

            db.SaveChanges();


            //Add new charges
			int compCount = system.Leases.Count;
			List<Component> MainComps = system.Components.Where(n => n.Type.Name != "Monitor").ToList();
			List<Component> Monitors = system.Components.Where(n => n.Type.Name == "Monitor").ToList();

			foreach (var comp in MainComps)
			{
				Lease currentLease = comp.CurrentLease;
				currentLease.BeginDate = Convert.ToDateTime(BegBillDate);
				currentLease.EndDate = Convert.ToDateTime(EndBillDate);

				int Term = comp.Leases.FirstOrDefault().Overhead.Term + 1; // Add 1 for vendor rates

				VendorRate VendorRate = db.VendorRates.Where(n => n.Term == Term && n.Type.Name == comp.Type.Name).OrderByDescending(n => n.BeginDate).FirstOrDefault();
				Charge MainCharge = new Charge { Price = Convert.ToDecimal(CPUCost), VendorRate = VendorRate, Type = comp.Type, Tax = db.Taxes.OrderByDescending(n => n.Timestamp).First() };
				currentLease.Charges.Add(MainCharge);

				if (!String.IsNullOrWhiteSpace(InsuranceCost))
				{
                    VendorRate InsuranceVendorRate = db.VendorRates.Where(n => n.Term == Term && n.Type.Name == "Insurance").OrderByDescending(n => n.BeginDate).FirstOrDefault();
					Charge InsuranceCharge = new Charge { Price = Convert.ToDecimal(InsuranceCost), VendorRate = InsuranceVendorRate, Type = db.TypesInsurance };
					currentLease.Charges.Add(InsuranceCharge);
				}

				if (!String.IsNullOrWhiteSpace(WarrantyCost))
				{
                    VendorRate WarrantyVendorRate = db.VendorRates.Where(n => n.Term == Term && n.Type.Name == "Insurance").OrderByDescending(n => n.BeginDate).FirstOrDefault();
					Charge WarrantyCharge = new Charge { Price = Convert.ToDecimal(WarrantyCost) / compCount, VendorRate = WarrantyVendorRate, Type = db.TypesWarranty };
					currentLease.Charges.Add(WarrantyCharge);
				}

				if (!String.IsNullOrWhiteSpace(ShippingCost))
				{
                    VendorRate ShippingVendorRate = db.VendorRates.Where(n => n.Term == Term && n.Type.Name == "Insurance").OrderByDescending(n => n.BeginDate).FirstOrDefault();
					Charge ShippingCharge = new Charge { Price = Convert.ToDecimal(ShippingCost) / compCount, VendorRate = ShippingVendorRate, Type = db.TypesShipping };
					currentLease.Charges.Add(ShippingCharge);
				}
			}
            int i = 0;
			foreach (var comp in Monitors)
			{
				Lease currentLease = comp.CurrentLease;
				currentLease.BeginDate = Convert.ToDateTime(BegBillDate);
				currentLease.EndDate = Convert.ToDateTime(EndBillDate);

				int Term = comp.Leases.FirstOrDefault().Overhead.Term + 1; // Add 1 for vendor rates

				VendorRate VendorRate = db.VendorRates.Where(n => n.Term == Term && n.Type.Name == comp.Type.Name).OrderByDescending(n => n.BeginDate).FirstOrDefault();

                Charge MainCharge;
                if (i == 0)
                {
                    MainCharge = new Charge { Price = Convert.ToDecimal(MonitorCost), VendorRate = VendorRate, Type = comp.Type, Tax = db.Taxes.OrderByDescending(n => n.Timestamp).First() };
                }
                else
                {
                    MainCharge = new Charge { Price = Convert.ToDecimal(Monitor2Cost), VendorRate = VendorRate, Type = comp.Type, Tax = db.Taxes.OrderByDescending(n => n.Timestamp).First() };
                }
                i++;
				currentLease.Charges.Add(MainCharge);
                
				if (!String.IsNullOrWhiteSpace(InsuranceCost) && MainComps.Count == 0)
				{
                    VendorRate InsuranceVendorRate = db.VendorRates.Where(n => n.Term == Term && n.Type.Name == "Insurance").OrderByDescending(n => n.BeginDate).FirstOrDefault();
					Charge InsuranceCharge = new Charge { Price = Convert.ToDecimal(InsuranceCost), VendorRate = InsuranceVendorRate, Type = db.TypesInsurance };
					currentLease.Charges.Add(InsuranceCharge);
				}

				if (!String.IsNullOrWhiteSpace(WarrantyCost))
				{
                    VendorRate WarrantyVendorRate = db.VendorRates.Where(n => n.Term == Term && n.Type.Name == "Insurance").OrderByDescending(n => n.BeginDate).FirstOrDefault();
					Charge WarrantyCharge = new Charge { Price = Convert.ToDecimal(WarrantyCost) / compCount, VendorRate = WarrantyVendorRate, Type = db.TypesWarranty };
					currentLease.Charges.Add(WarrantyCharge);
				}

				if (!String.IsNullOrWhiteSpace(ShippingCost))
				{
                    VendorRate ShippingVendorRate = db.VendorRates.Where(n => n.Term == Term && n.Type.Name == "Insurance").OrderByDescending(n => n.BeginDate).FirstOrDefault();
					Charge ShippingCharge = new Charge { Price = Convert.ToDecimal(ShippingCost) / compCount, VendorRate = ShippingVendorRate, Type = db.TypesShipping };
					currentLease.Charges.Add(ShippingCharge);
				}
			}

			db.SaveChanges();
			


		}

        public static decimal CalculateTax(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            var comp = db.Components.Where(n => n.Id == Id).Single();

            var charges = comp.Leases.OrderByDescending(n => n.EndDate).First().Charges;

            var tax = charges.Where(n => n.Type == comp.Type).Single().Tax;

            return (tax.Price * CalculateLeasingRate(Id));
        }

        public static decimal CalculateChargedRate(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            var comp = db.Components.Where(n => n.Id == Id).Single();

            var overhead = comp.Leases.OrderByDescending(n => n.EndDate).First().Overhead.Rate;

            return overhead;
        }

        public static decimal CalculateSecondaryCosts(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            var comp = db.Components.Where(n => n.Id == Id).Single();

            List<Charge> charges = comp.Leases.OrderByDescending(n => n.EndDate).First().Charges.ToList().Where(n => !ChargedComponent.Contains(n.Type.Name.ToUpper())).ToList();
            decimal term = comp.Leases.OrderByDescending(n => n.EndDate).First().Overhead.Term;

            decimal SecondaryCharges = 0M;
            foreach (var charge in charges)
            {
                SecondaryCharges += ((charge.VendorRate.Rate * charge.Price / 1000) * ((term + 1) / (term)));
            }

            return SecondaryCharges;
        }

        public static decimal CalculateSecondaryChargeRate(int Id, string TypeName)
        {
            AuleaseEntities db = new AuleaseEntities();
            var comp = db.Components.Where(n => n.Id == Id).Single();

            List<Charge> charges = comp.Leases.OrderByDescending(n => n.EndDate).First().Charges.ToList().Where(n => n.Type.Name == TypeName).ToList();

            if (charges.Count == 0)
            {
                return 0M;
            }

            decimal Rate = 0M;
            var charge = charges.Single();

            decimal term = comp.Leases.OrderByDescending(n => n.EndDate).First().Overhead.Term;

            Rate += ((charge.VendorRate.Rate * charge.Price / 1000) * ((term + 1) / (term)));
            
            return Rate;
        }

        public static decimal GetComponentCost(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            var comp = db.Components.Where(n => n.Id == Id).Single();

            return comp.Leases.OrderByDescending(n => n.EndDate).First().Charges.Where(n => n.Type == comp.Type).Single().Price;
        }

        public static decimal GetInsuranceCost(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            var comp = db.Components.Where(n => n.Id == Id).Single();
            aulease.Entities.Type Insurance = db.Types.Where(n => n.Name == "Insurance").Single();

            if (comp.Leases.OrderByDescending(n => n.EndDate).First().Charges.Where(n => n.Type == Insurance).Count() > 0)
            {
                return comp.Leases.OrderByDescending(n => n.EndDate).First().Charges.Where(n => n.Type == Insurance).First().Price;
            }
            else
            {
                return 0M;
            }
        }

        public static decimal GetWarrantyCost(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            var comp = db.Components.Where(n => n.Id == Id).Single();
            aulease.Entities.Type Insurance = db.Types.Where(n => n.Name == "Warranty").Single();

            if (comp.Leases.OrderByDescending(n => n.EndDate).First().Charges.Where(n => n.Type == Insurance).Count() > 0)
            {
                return comp.Leases.OrderByDescending(n => n.EndDate).First().Charges.Where(n => n.Type == Insurance).First().Price;
            }
            else
            {
                return 0M;
            }
        }

        public static decimal GetShippingCost(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            var comp = db.Components.Where(n => n.Id == Id).Single();
            aulease.Entities.Type Insurance = db.Types.Where(n => n.Name == "Shipping").Single();

            if (comp.Leases.OrderByDescending(n => n.EndDate).First().Charges.Where(n => n.Type == Insurance).Count() > 0)
            {
                return comp.Leases.OrderByDescending(n => n.EndDate).First().Charges.Where(n => n.Type == Insurance).First().Price;
            }
            else
            {
                return 0M;
            }
        }

        public static decimal GetIGFRate(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            var comp = db.Components.Where(n => n.Id == Id).Single();

            return comp.Leases.OrderByDescending(n => n.EndDate).First().Charges.Where(n => n.Type == comp.Type).Single().VendorRate.Rate;
        }

        public static decimal CalculateLeasingRate(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            Lease lease = db.Components.Where(n => n.Id == Id).Single().Leases.OrderByDescending(n => n.EndDate).FirstOrDefault();
            decimal MonthlyCharge = 0.00M;

            Charge charge = lease.Charges.Where(n => n.TypeId == lease.Component.TypeId).Single();

            decimal term = lease.Overhead.Term;

            decimal VendorRate = charge.VendorRate.Rate;
            MonthlyCharge = +MonthlyCharge + (charge.Price * VendorRate / 1000);

            
            return MonthlyCharge;
        }

        public static decimal GetIGFInsuranceRate(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            var comp = db.Components.Where(n => n.Id == Id).Single();

            List<Charge> charges = comp.Leases.OrderByDescending(n => n.EndDate).First().Charges.Where(n => n.Type != comp.Type && n.Price > 0).ToList();

            if (charges.Count == 0)
            {
                return 0.00M;
            }

            return charges.First().VendorRate.Rate;
        }
	}
}