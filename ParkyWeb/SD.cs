using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb
{
    public static class SD
    {
        public static string APIBaseURL = "http://localhost:49306/";
        public static string NationalParkAPIPath = APIBaseURL + "api/v1/nationalparks/";
        public static string TrailAPIPath = APIBaseURL + "api/v1/trails/";
        public static string AccountAPIPath = APIBaseURL + "api/v1/Users/";

    }
}
