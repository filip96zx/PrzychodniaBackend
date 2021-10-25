using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Przychodnia.Interfaces;
using Przychodnia.Models;
using Przychodnia.Transfer.User;
using Przychodnia.Transfer.Visit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PatientController : ControllerBase
    {
        private IVisitService _visitService;
        private readonly IUserService _userService;
        private UserManager<User> _userManager { get; set; }
        
        public PatientController(IVisitService VisitService, UserManager<User> userManager, IUserService userService)
        {
            _visitService = VisitService;
            _userManager = userManager;
            _userService = userService;
        }



        [HttpPost("GetVisits")]
        public async Task<IActionResult> GetVisit(GetVisitsDTO getvisit)
        {
           // var user = await _userManager.GetUserAsync(HttpContext.User);
            var result = await _visitService.GetVisitsInWeek(getvisit);
            return Ok(result);
        }

        [HttpPost("GetAllDoctorsVisits")]
        public async Task<IActionResult> GetAllDoctorsVisits(GetAllDoctorsVisitsDTO getvisit)
        {
            // var user = await _userManager.GetUserAsync(HttpContext.User);
            var result = await _visitService.GetVisitsInWeek(getvisit);
            return Ok(result);
        }

        [HttpPost("RegisterToVisit")]
        public async Task<IActionResult> RegisterToVisit(RegisterToVisitDTO visit)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = _visitService.RegisterToVisit(visit, user);
            return Ok(result);
        }

        [HttpPost("CancelVisitReservation")]
        public async Task<IActionResult> CancelVisitReservation(RegisterToVisitDTO visit)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = _visitService.CancelVisitReservation(visit, user);
            return Ok(result);
        }

        [HttpGet("GetDoctors")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _visitService.GetDoctorsTypes();

            return Ok(doctors);
        }

        [HttpPost("GetDoctorsInType")]
        [AllowAnonymous]
        public IActionResult GetDoctors(GetDoctorsDTO doctor)
        {
            var doctors = _visitService.GetDoctorsInType(doctor);

            return Ok(doctors);
        }

        [HttpGet("GetReservedVisits")]
        public async Task<IActionResult> GetReservedVisits([FromQuery] ListQuery query)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }

            var result = _visitService.PatientGetReservedVisits(query, user);

            return Ok(result.Value);
        }

        [HttpGet("GetDoneVisits")]
        public async Task<IActionResult> GetDoneVisits([FromQuery] ListQuery query)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }

            var result = _visitService.PatientGetDoneVisits(query, user);

            return Ok(result.Value);
        }
        [HttpPost("GetVisitDetails")]
        public async Task<IActionResult> GetVisitDetail(GetVisitDetailsDTO visit)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }
            visit.PatientId = user.Id.ToString();

            var result = await _visitService.PatientGetVisitDetails(visit);

            return Ok(result);
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage(SendMessageDTO message)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }

            var result = _visitService.PatientSendMessage(message, user);

            return Ok(result);
        }


        [HttpPost("GetMessages")]
        public async Task<IActionResult> GetMessages(GetVisitDetailsDTO visit)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }
            visit.PatientId = user.Id.ToString();

            var result = _visitService.PatientGetMessages(visit);

            return Ok(result);
        }

        [HttpPost("GetPrescritpions")]
        public async Task<IActionResult> GetPrescritpions(GetVisitDetailsDTO prescription)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }
            prescription.PatientId = user.Id.ToString();

            var result = _visitService.PatientGetPrescriptions(prescription);

            return Ok(result);
        }
        [HttpPost("GetFindings")]
        public async Task<IActionResult> GetFindings(GetVisitDetailsDTO finding)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }
            finding.PatientId = user.Id.ToString();

            var result = _visitService.PatientGetFindings(finding);

            return Ok(result);
        }

    }
}
