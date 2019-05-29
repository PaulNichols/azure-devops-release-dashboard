using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Dashboard
    {
        public List<Project> Projects { get; set; }

        public List<string> Environments
        {
            get
            {
                var namesWithHighestRank =
                        Projects.SelectMany(z => z.Environments)
                        .GroupBy(x => x.Name)
                        .Select(x => new KeyValuePair<string, int>
                                (
                                x.Key,
                                x.Max(y => y.Rank)
                                ))
                        .Where(x => !x.Key.ToLower().Contains("tests"))
                        .OrderBy(x => x.Value)
                        .Select(x=>x.Key)
                        .ToList();

                return namesWithHighestRank;
            }
        }
    }
}
