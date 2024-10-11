using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly StoreContext _dbContext;

        public BuggyController(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("notfound")]
        public ActionResult NotFoundRequest() // Get: api/Buggy/notfound
        {
            var product = _dbContext.Products.Find(100);
            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }
            return Ok(product);
        }

        [HttpGet("servererror")]
        public ActionResult GetServerError() // Get: api/Buggy/servererror
        {
            var product = _dbContext.Products.Find(100);
            var productDTO = product.ToString();
            return Ok(productDTO);
        }

        [HttpGet("badrequest")]
        public ActionResult GetBadRequest() // Get: api/Buggy/badrequest
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("unauthorized")]
        public ActionResult GetUnAuthorized() // Get: api/Buggy/unauthorized
        {
            return Unauthorized(new ApiResponse(401));
        }

        [HttpGet("badrequest/{id}")]
        public ActionResult GetBadRequest(int id) // Get: api/Buggy/badrequest/one
        {
            return Ok();
        }
    }
}
