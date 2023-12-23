﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Datalayer.Queries;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        public AccountController(ILogger<AccountController> logger)
        {
            this._logger = logger;
        }

        [HttpGet("GetByEmail/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Dictionary<string, string>>> GetAsync(string email)
        {
            if (email == null) { return BadRequest(); }
            Dictionary<string, string> account = await AccountQuery.ReadAccountByEmail(email);
            if (!account.ContainsKey("accounts_id")) { return NotFound(); }
            return Ok(account);
        }

        [HttpGet("GetEntireTable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Dictionary<string, string>>> GetEntireTable()
        {
            List<Dictionary<string, string>> table = await AccountQuery.ReadAccounts();
            if(table.Count < 0) return NotFound();
            return Ok(table);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<Dictionary<string,string>>> TryLogin([FromBody] Dictionary<string,string> user ) {
            if (user["accounts_email"] == null || user["accounts_password"] == null) return BadRequest();
            if (await AccountQuery.TryLogin(user["accounts_email"], user["accounts_password"])) {
                user = await AccountQuery.ReadAccountByEmail(user["accounts_email"]);
                return Ok(user);
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<Dictionary<string,string>>> TryRegister([FromBody] Dictionary<string,string> user) {
            if (user["accounts_email"] == null || user["accounts_password"] == null) return BadRequest();
            else if (AccountQuery.ReadAccountByEmail(user["accounts_email"]) != null) return BadRequest();
            else AccountQuery.CreateAccount(user);
            return Ok(AccountQuery.ReadAccountByEmail(user));
        }
    }
}
