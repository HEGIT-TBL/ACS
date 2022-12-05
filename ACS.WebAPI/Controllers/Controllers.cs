using ACS.Core.Contracts.Services;
using ACS.Core.Models;
using ACS.Core.Models.Events;
using ACS.Core.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ACS.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenericController<T> : ControllerBase
        where T : class, IHasGuid
    {
        private readonly IGenericRepositoryAsync<T> _genericRepository;

        public GenericController(IGenericRepositoryAsync<T> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        [HttpPost]
        [Route("Attach")]
        public void Attach([FromBody] T item)
        {
            _genericRepository.Attach(item);
        }

        [HttpPost]
        [Route("GetAllAsync")]
        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _genericRepository.GetAllAsync(cancellationToken);
        }

        [HttpPost]
        [Route("GetOneAsync")]
        public async Task<T> GetOneAsync([FromBody] Guid id, CancellationToken cancellationToken)
        {
            return await _genericRepository.GetOneAsync(id, cancellationToken);
        }

        [HttpPost]
        [Route("CreateAsync")]
        public async Task CreateAsync([FromBody] T item, CancellationToken cancellationToken)
        {
            await _genericRepository.CreateAsync(item, cancellationToken);
        }

        [HttpPost]
        [Route("Update")]
        public void Update([FromBody] T otherItem)
        {
            _genericRepository.Update(otherItem);
        }

        [HttpPost]
        [Route("Delete")]
        public void Delete([FromBody] T item)
        {
            _genericRepository.Delete(item);
        }

        [HttpPost]
        [Route("DeleteAsync")]
        public async Task DeleteAsync([FromBody] Guid id, CancellationToken cancellationToken)
        {
            await _genericRepository.DeleteAsync(id, cancellationToken);
        }

        [HttpPost]
        [Route("SaveChangesAsync")]
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _genericRepository.SaveChangesAsync(cancellationToken);
        }

        [HttpPost]
        [Route("ElementExistsAsync")]
        public async Task<bool> ElementExistsAsync([FromBody] Guid id, CancellationToken cancellationToken)
        {
            return await _genericRepository.ElementExistsAsync(id, cancellationToken);
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class UserController : GenericController<User>
    {
        public UserController(IGenericRepositoryAsync<User> genericRepository) : base(genericRepository) { }
    }

    [ApiController]
    [Route("[controller]")]
    public class KeyCardController : GenericController<KeyCard>
    {
        public KeyCardController(IGenericRepositoryAsync<KeyCard> genericRepository) : base(genericRepository) { }
    }

    [ApiController]
    [Route("[controller]")]
    public class AccessPointController : GenericController<AccessPoint>
    {
        public AccessPointController(IGenericRepositoryAsync<AccessPoint> genericRepository) : base(genericRepository) { }
    }

    [ApiController]
    [Route("[controller]")]
    public class CameraController : GenericController<Camera>
    {
        public CameraController(IGenericRepositoryAsync<Camera> genericRepository) : base(genericRepository) { }
    }

    [ApiController]
    [Route("[controller]")]
    public class ParkingLotController : GenericController<ParkingLot>
    {
        public ParkingLotController(IGenericRepositoryAsync<ParkingLot> genericRepository) : base(genericRepository) { }
    }

    [ApiController]
    [Route("[controller]")]
    public class AccessEventController : GenericController<AccessEvent>
    {
        public AccessEventController(IGenericRepositoryAsync<AccessEvent> genericRepository) : base(genericRepository) { }
    }

    [ApiController]
    [Route("[controller]")]
    public class FaceRecognizedEventController : GenericController<FaceRecognizedEvent>
    {
        public FaceRecognizedEventController(IGenericRepositoryAsync<FaceRecognizedEvent> genericRepository) : base(genericRepository) { }
    }

    [ApiController]
    [Route("[controller]")]
    public class ParkingLotStateChangedEventController : GenericController<ParkingLotStateChangedEvent>
    {
        public ParkingLotStateChangedEventController(IGenericRepositoryAsync<ParkingLotStateChangedEvent> genericRepository) : base(genericRepository) { }
    }

    [ApiController]
    [Route("[controller]")]
    public class CarController : GenericController<Car>
    {
        public CarController(IGenericRepositoryAsync<Car> genericRepository) : base(genericRepository) { }
    }
}