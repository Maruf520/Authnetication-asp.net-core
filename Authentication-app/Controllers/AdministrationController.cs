using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authentication_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authentication_app.Controllers
{
    [Authorize]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName,
                };
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListRole", "Administration");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }
            return View(model);

        }
        public IActionResult ListRole()
        {
            var role = roleManager.Roles;
            return View(role);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            
            var role = await roleManager.FindByIdAsync(id);
/*            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found ";
                return View("NotFound");
            }*/
            var model = new EditViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
            };
            foreach(var user in userManager.Users)
            {
                if(await userManager.IsInRoleAsync(user,role.Name))
                {
                    model.Users.Add(user.UserName);
                }

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditViewModel editViewModel)
        {

            var role = await roleManager.FindByIdAsync(editViewModel.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {editViewModel.Id} cannot be found ";
                return View("NotFound");
            }
            else
            {
                role.Name = editViewModel.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListRole"); 
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }
            return View(editViewModel);

        }
        [HttpGet]
        public async Task<IActionResult>EditeUserInRole(string roleId)
            {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with this Id = {roleId} cannot be found";
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();
            foreach(var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                if (await userManager.IsInRoleAsync(user,role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;

                }
                model.Add(userRoleViewModel);
            }
            return View(model);
            }

        [HttpPost]
        public async Task<IActionResult> EditeUserInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot ne found";
                return View("NotFound");
            }
            for(int i = 0;i<model.Count;i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name))) 
                {
                    userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user,role.Name);
                }
                else
                {
                    continue;
                }
                /*if(result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole",new { Id = roleId });
                    }
                }*/
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
    }

    }
