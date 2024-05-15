using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TmkGondorTreasury.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // GET: api/users
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            // TODO: Implement logic to retrieve users from the database or any other data source
            var users = new List<string> { "User1", "User2", "User3" };
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public ActionResult<string> GetById(int id)
        {
            // TODO: Implement logic to retrieve a specific user by ID from the database or any other data source
            var user = $"User with ID {id}";
            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public ActionResult<string> Create([FromBody] string user)
        {
            // TODO: Implement logic to create a new user in the database or any other data source
            // You can access the user data from the request body using the [FromBody] attribute
            // Example: var newUser = JsonConvert.DeserializeObject<User>(user);
            var createdUser = $"User created: {user}";
            return Created("", createdUser);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public ActionResult<string> Update(int id, [FromBody] string user)
        {
            // TODO: Implement logic to update an existing user in the database or any other data source
            // You can access the user data from the request body using the [FromBody] attribute
            // Example: var updatedUser = JsonConvert.DeserializeObject<User>(user);
            var updatedUser = $"User updated with ID {id}: {user}";
            return Ok(updatedUser);
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public ActionResult<string> Delete(int id)
        {
            // TODO: Implement logic to delete an existing user from the database or any other data source
            var deletedUser = $"User deleted with ID {id}";
            return Ok(deletedUser);
        }
    }
}