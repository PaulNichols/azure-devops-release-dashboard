using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class ReleaseEnvironmentInfo
    {

        public string Name { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "-")]
        public string Version { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "-")]
        public int Rank { get; set; }

        public enum StatusEnum
        {
            Undefined = 1,

            NotStarted = 2,

            InProgress = 3,

            Succeeded = 4,

            Canceled = 5,

            Rejected = 6,

            Queued = 7,

            Scheduled = 8,

            PartiallySucceeded = 9
        }

        public string GetStatusText()
        {
            return Version ?? "N/A"; ;
            //switch (Status)
            //{
            //    case StatusEnum.Succeeded:
            //        return "Succeeded";
            //    case StatusEnum.InProgress:
            //        return "Deploying";
            //    case StatusEnum.Canceled:
            //        return "Cancelled";
            //    case StatusEnum.Rejected:
            //        return "Failed";
            //    case StatusEnum.Undefined:
            //        return "Not Deployed";
            //    case StatusEnum.PartiallySucceeded:
            //        return "Partially Succeeded";
            //    case StatusEnum.NotStarted:
            //        return "Not Deployed";
            //    case StatusEnum.Queued:
            //        return "Deploying";
            //    default:
            //        return "Not Deployed";
            //}
        }

        public StatusEnum? Status { get; set; }


        public string StatusText
        {
            get
            {
                return Humanizer.StringHumanizeExtensions.Humanize(Status.ToString(), Humanizer.LetterCasing.Title);
            }
        }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "-")]
        public string Who { get; set; }


        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "-")]
        public string When { get; set; }

        public string StatusClass
        {
            get
            {
                switch (Status)
                {
                    case StatusEnum.Succeeded:
                        return "button-green";
                    case StatusEnum.InProgress:
                        return "button-blue";
                    case StatusEnum.Canceled:
                    case StatusEnum.Rejected:
                        return "button-red";
                    case StatusEnum.PartiallySucceeded:
                        return "button-orange";
                    case StatusEnum.Scheduled:
                        return "button-cyan";
                    case StatusEnum.Queued:
                        return "button-yellow";
                    default:
                        return "button-gray";
                }
            }
        }

        public int ReleaseId { get; internal set; }
    }
}
