// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ClinicaVistaalegre.Server.Data;
using ClinicaVistaalegre.Server.Models;
using ClinicaVistaalegre.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicaVistaalegre.Server.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private ApplicationDbContext _dbContext;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Número de teléfono")]
            public string PhoneNumber { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Apellidos")]
            public string Apellidos { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "FechaNacimiento")]
            public DateTime FechaNacimiento { get; set; }

            [Display(Name = "Sexo")]
            public char Sexo { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Especialidad")]
            public string Especialidad { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Apellidos = user.Apellidos,
                Especialidad = user.Especialidad,
                FechaNacimiento = (DateTime)user.FechaDeNacimiento,
                Sexo = (char)user.Sexo
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Error al cargar usuario con id: '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userPaciente = new Paciente();
            var userMedico = new Medico();
            if (user.Especialidad.Equals("Paciente"))
            {
                userPaciente = _dbContext.Pacientes.Where(x => x.Id == user.Id).FirstOrDefault();
            }
            else
            {
                userMedico = _dbContext.Medicos.Where(x => x.Id == user.Id).FirstOrDefault();
            }

            if (user == null)
            {
                return NotFound($"Error al cargar usuario con id: '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Error inesperado.";
                    return RedirectToPage();
                }
            }

            if (Input.Apellidos != user.Apellidos)
            {
                try
                {
                    user.Apellidos = Input.Apellidos;
                    _dbContext.Update(user);
                    if (user.Especialidad.Equals("Paciente"))
                    {
                        userPaciente.Apellidos = Input.Apellidos;
                        _dbContext.Update(userPaciente);
                    }
                    else
                    {
                        userMedico.Apellidos = Input.Apellidos;
                        _dbContext.Update(userMedico);
                    }
                    _dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    StatusMessage = "Error inesperado.";
                    RedirectToPage();
                }
            }

            if (Input.Especialidad != user.Especialidad && user.Especialidad != "Paciente")
            {
                try
                {
                    user.Especialidad = Input.Especialidad;
                    _dbContext.Update(user);

                    userMedico.Especialidad = Input.Especialidad;
                    _dbContext.Update(userMedico);

                    _dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    StatusMessage = "Error inesperado.";
                    RedirectToPage();
                }
            }

            if (Input.FechaNacimiento != user.FechaDeNacimiento && user.Especialidad == "Paciente")
            {
                try
                {
                    user.FechaDeNacimiento = (DateTime)Input.FechaNacimiento;
                    _dbContext.Update(user);
                    if (user.Especialidad.Equals("Paciente"))
                    {
                        userPaciente.FechaDeNacimiento = Input.FechaNacimiento;
                        _dbContext.Update(userPaciente);
                    }
                    _dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    StatusMessage = "Error inesperado.";
                    RedirectToPage();
                }
            }

            if (Input.Sexo != user.Sexo && user.Especialidad != "Medico")
            {
                try
                {
                    user.Sexo = Input.Sexo;
                    _dbContext.Update(user);
                    if (user.Especialidad.Equals("Paciente"))
                    {
                        userPaciente.Sexo = Input.Sexo;
                        _dbContext.Update(userPaciente);
                    }
                    _dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    StatusMessage = "Error inesperado.";
                    RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Su perfil ha sido actualizado";
            return RedirectToPage();
        }
    }
}
