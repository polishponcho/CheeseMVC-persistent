using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Data;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            List<Menu> menus = context.Menus.ToList();
            return View(menus);
        }

        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if (ModelState.IsValid)
            {
                Menu newMenu = new Menu()
                {
                    Name = addMenuViewModel.Name
                };
                context.Menus.Add(newMenu);
                context.SaveChanges();

                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }
            return View(addMenuViewModel);
        }
        public IActionResult ViewMenu(int id)
        {
            Menu viewMenu = context.Menus.Single(c => c.ID == id);
            List<CheeseMenu> items = context
                .CheeseMenus
                .Include(item => item.Cheese)
                .Where(cm => cm.MenuID == id)
                .ToList();
            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel()
            {
                Menu = viewMenu,
                Items = items
            };
            return View(viewMenuViewModel);

        }

        public IActionResult AddItem(int id)
        {
            Menu newMenuItem = context.Menus.Single(m => m.ID == id);
            List<Cheese> cheeses = context.Cheeses.ToList();
            AddMenuItemViewModel addMenuItemViewModel =
                new AddMenuItemViewModel(newMenuItem, cheeses);
            return View(addMenuItemViewModel);
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            if(ModelState.IsValid)
            {
                IList<CheeseMenu> existingItems = context.CheeseMenus
                    .Where(cm => cm.CheeseID == addMenuItemViewModel.cheeseID)
                    .Where(cm => cm.MenuID == addMenuItemViewModel.menuID).ToList();

                if(existingItems.Count == 0)
                {
                    CheeseMenu newCheeseMenu = new CheeseMenu()
                    {
                        MenuID = addMenuItemViewModel.menuID,
                        CheeseID = addMenuItemViewModel.cheeseID
                    };
                    context.CheeseMenus.Add(newCheeseMenu);
                    context.SaveChanges();

                    return Redirect("/Menu/ViewMenu/" + newCheeseMenu.MenuID);



                }
                return Redirect("/Menu/Index");
            }
            return View(addMenuItemViewModel);
        }
    }
}
