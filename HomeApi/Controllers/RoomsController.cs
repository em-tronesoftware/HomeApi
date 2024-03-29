﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using HomeApi.Contracts.Models.Rooms;
using HomeApi.Data.Models;
using HomeApi.Data.Queries;
using HomeApi.Data.Repos;
using Microsoft.AspNetCore.Mvc;

namespace HomeApi.Controllers
{
    /// <summary>
    /// Контроллер комнат
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : ControllerBase
    {
        private IRoomRepository _rooms;
        private IMapper _mapper;
        
        public RoomsController(IRoomRepository rooms, IMapper mapper)
        {
            _rooms = rooms;
            _mapper = mapper;
        }
        
        //TODO: Задание - добавить метод на получение всех существующих комнат
        
        /// <summary>
        /// Добавление комнаты
        /// </summary>
        [HttpPost] 
        [Route("")] 
        public async Task<IActionResult> Add([FromBody] AddRoomRequest request)
        {
            var existingRoom = await _rooms.GetRoomByName(request.Name);
            if (existingRoom == null)
            {
                var newRoom = _mapper.Map<AddRoomRequest, Room>(request);
                await _rooms.AddRoom(newRoom);
                return StatusCode(201, $"Комната {request.Name} добавлена!");
            }
            
            return StatusCode(409, $"Ошибка: Комната {request.Name} уже существует.");
        }
        
        /// <summary>
        /// обновление существующей комнаты
        /// </summary>
        [HttpPut] 
        [Route("{id}")] 
        public async Task<IActionResult> Update (
            [FromRoute] Guid id,
            [FromBody] UpdateRoomRequest request)
        {
            // Ищем комнату по Id (внутренняя валидация на стороне Базы Данных)
            var room = await _rooms.GetRoomById(id);
            if(room == null)
                return StatusCode(400, $"Ошибка: Комната с идентификатором {id} не найдена.");

            // Обновляем через репозиторий
            await _rooms.UpdateRoom(room, new UpdateRoomQuery(
                request.Name,
                request.Area,
                request.GasConnected,
                request.Voltage
            ));
            
            // Возарвщаем результат
            return StatusCode(200, $"Комната обновлена!");
        }
    }
}