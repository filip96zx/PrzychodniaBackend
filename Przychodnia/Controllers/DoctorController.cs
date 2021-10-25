
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Przychodnia.Interfaces;
using Przychodnia.Models;
using Przychodnia.Transfer.Visit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Przychodnia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class DoctorController : ControllerBase
    {
        private IVisitService _visitService;
        private UserManager<User> _userManager { get; set; }

        public DoctorController(IVisitService VisitService, UserManager<User> userManager)
        {
            _visitService = VisitService;
            _userManager = userManager;
        }

        [HttpPost("CreateVisit")]
        public async Task<IActionResult> CreateVisit(CreateVisitListDTO visit)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
                return BadRequest(ModelState);
            }
            var result = await _visitService.CreateVisit(visit, user);
            return Ok(result);
        }

        [HttpGet("GetTypes")]
        public async Task<IActionResult> GetTypes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
                return BadRequest(ModelState);
            }
            var result = await _visitService.DoctorGetTypes(user);
            return Ok(result);
        }

        [HttpPost("GetVisits")]
        public async Task<IActionResult> GetVisit(DoctorGetVisitsDTO getvisit)
        {
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
                return BadRequest(ModelState);
            }
            GetVisitsDTO visit = new GetVisitsDTO
            {
                WeekDay = getvisit.WeekDay,
                DoctorId = user.Id.ToString()
            };
            var result = await _visitService.DoctorGetVisitsInWeek(visit);
            return Ok(result);
        }

        [HttpDelete("DeleteVisit")]
        public async Task<IActionResult> DeleteVisit(DeleteVisitDTO visit)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
                return BadRequest(ModelState);
            }

            var result = _visitService.DeleteVisit(visit, user);

            if (!result.Success)
            {
                ModelState.AddModelError("errorMessage", result.ErrorMessage);
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpPost("GetVisitDetails")]
        public async Task<IActionResult> GetVisitDetail(DoctorGetVisitDetailsDTO visit)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }

            var result = await _visitService.DoctorGetVisitDetails(visit, user);

            return Ok(result);
        }

        [HttpPost("FinishVisit")]
        public async Task<IActionResult> FinishVisit(DoctorGetVisitDetailsDTO visit)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }

            var result = _visitService.DoctorFinishVisit(visit, user);

            return Ok(result);
        }

        [HttpPost("CancelVisit")]
        public async Task<IActionResult> CancelVisit(DoctorGetVisitDetailsDTO visit)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }

            var result = _visitService.DoctorCancelVisit(visit, user);

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

            var result = _visitService.DoctorSendMessage(message, user);

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
            visit.DoctorId = user.Id.ToString();

            var result = _visitService.DoctorGetMessages(visit);

            return Ok(result);
        }

        [HttpPost("SendPrescritpion")]
        public async Task<IActionResult> SendPrescritpion(SendPrescriptionDTO prescritpion)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }

            var result = _visitService.DoctorSendPrescription(prescritpion, user);

            return Ok(result);
        }

        [HttpPost("GetPrescritpions")]
        public async Task<IActionResult> GetPrescritpion(GetVisitDetailsDTO visit)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }
            visit.DoctorId = user.Id.ToString();

            var result = _visitService.DoctorGetPrescriptions(visit);

            return Ok(result);
        }

        [HttpPost("DeletePrescritpion")]
        public async Task<IActionResult> DeletePrescritpion(DeletePrescriptionDTO prescription)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }

            var result = _visitService.DoctorDeletePrescription(prescription, user);

            return Ok(result);
        }

        [HttpPost("SendFinding")]
        public async Task<IActionResult> SendFinding(NewFindingDTO finding)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }

            var result = _visitService.DoctorSendFinding(finding, user);

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
            finding.DoctorId = user.Id.ToString();

            var result = _visitService.DoctorGetFindings(finding);

            return Ok(result);
        }

        [HttpPost("DeleteFinding")]
        public async Task<IActionResult> DeleteFinding(DeleteFindingDTO finding)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("errorMessage", "Unauthorised user.");
            }

            var result = _visitService.DoctorDeleteFinding(finding, user);

            return Ok(result);
        }


    }
}
