namespace Body4U.Web.Areas.Administration.Controllers
{
    using Body4U.Common;
    using Body4U.Data;
    using Body4U.Data.Models;
    using Body4U.Web.Areas.Administration.ViewModels;
    using cloudscribe.Pagination.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DashboardController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ApplicationDbContext dbContext;

        public DashboardController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpGet]
        public IActionResult GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            var users = userManager
                .Users
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new GetAllUsersViewModel
                {
                    Id = x.Id,
                    Name = x.FullName,
                    Email = x.Email,
                    Roles = x.Roles
                        .Select(y => new GetAllUserRolesViewModel
                        {
                            Id = y.RoleId
                        })
                        .ToList()
                })
                .Skip((pageSize * pageNumber) - pageSize)
                .Take(pageSize)
                .AsQueryable();

            foreach (var user in users)
            {
                foreach (var role in user.Roles)
                {
                    role.RoleName = roleManager.FindByIdAsync(role.Id).Result.Name;
                }
            }

            var result = new PagedResult<GetAllUsersViewModel>
            {
                Data = users.AsNoTracking().ToList(),
                TotalItems = dbContext.Users.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(result);
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = roleManager.Roles.ToList();

            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> UsersInRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            var result = new UsersInRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    result.Users.Add(user.UserName);
                }
            }

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            var result = new List<EditUsersInRoleViewModel>();

            foreach (var user in userManager.Users)
            {
                var userRoleViewModel = new EditUsersInRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                result.Add(userRoleViewModel);
            }

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<EditUsersInRoleViewModel> model, string roleId)
        {
            //Търсим ролата по Id
            var role = await roleManager.FindByIdAsync(roleId);

            //Извъртаме всички потребители, които са били маркирани 
            for (int i = 0; i < model.Count; i++)
            {
                //Вземаме текущия потребител
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;
                
                //Ако е селектиран и не е в тази роля...
                if (model[i].IsSelected && !await userManager.IsInRoleAsync(user, role.Name))
                {
                    //Го добавяме
                    result = await userManager.AddToRoleAsync(user, role.Name);
                    if (role.Name == GlobalConstants.TrainerRoleName)
                    {
                        var trainer = new Trainer
                        {
                            ApplicationUserId = user.Id
                        };

                        dbContext.Trainers.Add(trainer);
                    }
                }
                //Ако не е селектиран и е в тази роля...
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    if (role.Name == GlobalConstants.TrainerRoleName)
                    {
                        var trainer = dbContext.Trainers.FirstOrDefault(x => x.ApplicationUserId == user.Id);
                        dbContext.Trainers.Remove(trainer);
                    }

                    //и премахваме от ролята
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        dbContext.SaveChanges();
                        return RedirectToAction("UsersInRole", new { Id = roleId });
                    }
                }
            }

            dbContext.SaveChanges();
            return RedirectToAction("UsersInRole", new { Id = roleId });
        }
    }
}
