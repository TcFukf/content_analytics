using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// все почти классы отсюда должны убраны вместо них готовые решения из .net но пока лень''''''''''''''
namespace social_analytics.Helpers
{
    public class ConfigHelper
    {
        static public string ConnectionString { get; set; } = "Host=localhost;Username=postgres;Password=kafka123;Database=mess_analytics";
    }
}
