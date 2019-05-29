using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IO.Swagger.Models.ReleaseEnvironmentInfo;

namespace WebApplication.Models
{
    public class Project
    {

        public string ProjectName { get; set; }

        public List<ReleaseEnvironmentInfo> Environments { get; set; }
        public ReleaseEnvironmentInfo GetEnvDetails(string env)
        {
            var envDetails = Environments.SingleOrDefault(x => x.Name == env);

            if (envDetails == null)
            {
                envDetails = new ReleaseEnvironmentInfo
                {
                    Status = ReleaseEnvironmentInfo.StatusEnum.Undefined,
                };
            }

            return envDetails;
        }
    }
}
