using System.Collections.Generic;

namespace CheeseMVC.Models
{
    public class Menu
    {
        public string Name { get; set; }

        public int ID { get; set; }

        public IList<CheeseMenu> CheeseMenus { get; set; }

    }

}
