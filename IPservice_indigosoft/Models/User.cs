using System.Collections;

namespace IPservice_indigosoft.Models
{
    public class User
    {
        required public long Id { get; set; }

        public ICollection<UserConnection> Connections { get; set; }
    }
}
