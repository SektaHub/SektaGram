﻿using AutoMapper;
using backend.Models.Dto;
using backend.Models.Entity;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using Pgvector;
using backend.Controllers.Common;
using Microsoft.AspNetCore.JsonPatch;
using System.Globalization;
using System;
using MongoDB.Bson;
using MongoDB.Driver;
using backend.Repo;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : BaseFileContentController<Image, ImageDto, ImageService>
    {


        public ImageController(IMapper mapper, IWebHostEnvironment webHostEnvironment, ILogger<BaseFileContentController<Image, ImageDto, ImageService>> logger, ImageService fileConentService) : base(mapper, webHostEnvironment, logger, fileConentService)
        {

        }

        //[HttpGet("{id}/GetConceptuallySimmilarImages")]
        //public async Task<IEnumerable<ImageDto>> GetConceptuallySimmilarImages(string id)
        //{

        //    var entity = _dbContext.Set<Image>().Find(id);

        //    if (entity == null)
        //    {
        //        return (IEnumerable<ImageDto>)NotFound();
        //    }

        //    var imageDto = _mapper.Map<ImageDto>(entity);

        //    var entities = await _dbContext.Set<Image>()
        //        .Where(x => x.Id != id)
        //        .OrderBy(x => x.CaptionEmbedding!.L2Distance(imageDto.CaptionEmbedding))
        //        .Take(4)
        //        .ToListAsync();

        //    var dtoList = _mapper.Map<List<ImageDto>>(entities);
        //    return dtoList;
        //}

        [HttpGet("GetImagesByCaption")]
        public IQueryable<ImageDto> GetImagesByCaption(string caption)
        {
            return _fileConentService.GetImagesByCaption(caption);
        }

        [HttpGet("GetImagesWithoutCaption")]
        public IQueryable<ImageDto> GetImagesWithoutCaption()
        {
            return _fileConentService.GetImagesWithoutCaption();
        }


        [HttpGet("{id}/Content2", Name = "GetImageStream2")]
        public async Task<IActionResult> GetFileContent2(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var imageEntity = _fileConentService.GetById(id);

                if (imageEntity == null)
                {
                    return NotFound();
                }

                var imageStream = await _fileConentService.GetFileStreamAsync(id);

                if (imageStream.Length == 0)
                {
                    return NotFound();
                }

                // Return the image stream
                return File(imageStream, $"image/{imageEntity.FileExtension}"); // Adjust the content type based on your image format
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while attempting to read the image file: {ex.Message}");
                return StatusCode(500, "An error occurred while attempting to read the image file.");
            }
        }


        //[HttpPost("upload-multiple")]
        //public async override Task<IActionResult> UploadMultiple(List<IFormFile> files)
        //{

        //    var imageDtos = await _fileConentService.UploadImage(files);

        //    return Ok(new { Message = "Images uploaded and saved successfully", UploadedFiles = imageDtos });
        //}

        [HttpDelete("2/{imageId}")]
        public async Task<IActionResult> DeleteFileContent2(string imageId)
        {
            try
            {
                await _fileConentService.DeleteImage(imageId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting image: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the image");
            }
        }

        [HttpPatch("{fileId}/PatchCaptionEmbedding")]
        public IActionResult Patch(string fileId, EmbeddingDto embedding)
        {
            if (embedding == null)
            {
                return BadRequest("Invalid embedding");
            }

            var existingEntity = _fileConentService.GetById(fileId);

            if (existingEntity == null)
            {
                return NotFound();
            }

            // Map the existing entity to a DTO for patching
            var dtoToPatch = _mapper.Map<ImageDto>(existingEntity);

            //List<float> embed = embedding.Embedding;

            Vector emb = new Vector(embedding.Embedding.Replace(" ", ""));

            dtoToPatch.CaptionEmbedding = emb;

            // Update entity properties based on the patched DTO
            _mapper.Map(dtoToPatch, existingEntity);

            // Perform the update in the database
            _fileConentService.Update();

            // Additional processing or actions after successful patch

            return Ok(dtoToPatch);
        }

    }
}
