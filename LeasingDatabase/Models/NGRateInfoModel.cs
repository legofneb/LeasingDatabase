using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
    public class NGRateInfoModel
    {
        public string Type { get; set; }
        public int Term { get; set; }
        public NGBillingRateModel CurrentRate { get; set; }
        public NGBillingRateModel PreviousRate { get; set; }

        public static List<NGRateInfoModel> GetRates(AuleaseEntities db)
        {
            List<NGRateInfoModel> Rates = new List<NGRateInfoModel>();

            List<VendorRate> RatesFromDB = db.VendorRates.ToList();

            foreach (aulease.Entities.Type Type in RatesFromDB.Select(n => n.Type).Distinct())
            {
                foreach (int Term in RatesFromDB.Select(n => n.Term).Distinct())
                {
                    if (!RatesFromDB.Any(n => n.Type == Type && n.Term == Term))
                    {
                        continue;
                    }

                    NGRateInfoModel RateInfoModel = new NGRateInfoModel();
                    RateInfoModel.Type = Type.Name;
                    RateInfoModel.Term = Term;

                    List<VendorRate> FilteredRates = RatesFromDB.Where(n => n.Type == Type && n.Term == Term).ToList();

                    VendorRate CurrentVendorRate  = FilteredRates.OrderByDescending(n => n.BeginDate).Take(1).Single();

                    NGBillingRateModel CurrentRateModel = new NGBillingRateModel();
                    CurrentRateModel.Month = CurrentVendorRate.BeginDate.ToString("MMM");
                    CurrentRateModel.Year = CurrentVendorRate.BeginDate.ToString("yyyy");
                    CurrentRateModel.Rate = CurrentVendorRate.Rate;
                    RateInfoModel.CurrentRate = CurrentRateModel;

                    if (CheckForPreviousRate(FilteredRates))
                    {
                        VendorRate PreviousVendorRate = FilteredRates.OrderByDescending(n => n.BeginDate).Skip(1).Take(1).Single();
                        NGBillingRateModel PreviousRateModel = new NGBillingRateModel();
                        PreviousRateModel.Month = PreviousVendorRate.BeginDate.ToString("MMM");
                        PreviousRateModel.Year = PreviousVendorRate.BeginDate.ToString("yyyy");
                        PreviousRateModel.Rate = PreviousVendorRate.Rate;
                        RateInfoModel.PreviousRate = PreviousRateModel;
                    }

                    Rates.Add(RateInfoModel);
                }
            }

            return Rates;
        }

        private static bool CheckForPreviousRate(List<VendorRate> FilteredRates)
        {
            return FilteredRates.Count > 1;
        }
    }

    public class NGBillingRateModel
    {
        public decimal Rate { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
    }
}