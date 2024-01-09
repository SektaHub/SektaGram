﻿using AutoMapper;
using backend.Models.Dto;
using backend.Models.Entity;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReelController : ControllerBase
    {

        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ReelController> _logger;
        private readonly ReelService _reelService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ReelController(ILogger<ReelController> logger, ReelService reelService, ApplicationDbContext dbContext, IMapper mapper, IWebHostEnvironment env)
        {
            _logger = logger;
            _reelService = reelService;
            _dbContext = dbContext;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet(Name = "Get")]
        public IEnumerable<ReelDto> Get()
        {
            var entities = _dbContext.Set<Reel>().ToList();
            var dtoList = _mapper.Map<List<ReelDto>>(entities);
            return dtoList;
        }

        [HttpGet("{videoId}", Name = "GetReelStream")]
        public IActionResult GetReel(Guid videoId)
        {
            var videoPath = _reelService.GetReelPath(videoId);

            if (string.IsNullOrEmpty(videoPath))
            {
                return NotFound();
            }

            try
            {
                var videoStream = new FileStream(videoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(videoStream, "video/mp4", enableRangeProcessing: true);
            }
            catch (IOException)
            {
                return StatusCode(500, "An error occurred while attempting to read the video file.");
            }
        }

        [HttpPost("generateThumbnail")]
        public async Task<IActionResult> GenerateThumbnail(Guid videoId)
        {
            var videoPath = _reelService.GetReelPath(videoId);
            var outputPath = Path.Combine(_env.WebRootPath, "Thumbnails");

            if (string.IsNullOrEmpty(videoPath))
            {
                return NotFound();
            }

            try
            {
                // Extract thumbnail using FFmpeg
                var thumbnailPath = await _reelService.ExtractThumbnailAsync(videoPath, outputPath);

                if (string.IsNullOrEmpty(thumbnailPath))
                {
                    // If thumbnail extraction fails, return an error response
                    return BadRequest("Failed to generate thumbnail");
                }

                // If thumbnail extraction succeeds, return the generated thumbnail path
                return Ok(new { ThumbnailPath = thumbnailPath });
            }
            catch (IOException)
            {
                return StatusCode(500, "An error occurred while attempting to read the video file.");
            }
        }

        [HttpGet("{videoId}/thumbnail", Name = "GetReelThumbnail")]
        public IActionResult GetReelThumbnail(Guid videoId)
        {
            var videoPath = _reelService.GetReelPath(videoId);
            var outputPath = Path.Combine(_env.WebRootPath, "Thumbnails");

            if (string.IsNullOrEmpty(videoPath))
            {
                return NotFound();
            }

            // Assume the thumbnail file has the same name as the video with a .jpg extension
            var thumbnailFileName = $"{videoId}.jpg";
            var thumbnailPath = Path.Combine(outputPath, thumbnailFileName);

            if (System.IO.File.Exists(thumbnailPath))
            {
                // If the thumbnail exists, return it
                try
                {
                    // Open the file stream without closing it immediately
                    var thumbnailStream = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    // Return the file stream directly as the response
                    return File(thumbnailStream, "image/jpeg");
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                    Console.WriteLine($"Error accessing thumbnail file: {ex.Message}");
                    return StatusCode(500, "An error occurred while attempting to read the thumbnail file.");
                }
            }
            else
            {
                // If the thumbnail doesn't exist, return a default placeholder or handle it as needed
                return NotFound("Thumbnail not found");
            }
        }


        [HttpPost("/api/DownloadFFmpeg", Name = "DownloadFFmpeg")]
        public IActionResult DownloadFFmpeg(Guid videoId)
        {
            //var videoPath = _reelService.GetReelPath(videoId);
            var outputPath = "C:\\Users\\Borjan\\Documents\\GitHub\\SektaGram\\backend\\wwwroot\\Thumbnails\\";
            _ = _reelService.DownloadFFmpeg();
            return Ok("FFmpeg dowloaded in : "+ outputPath);
        }

        [HttpGet("RandomVideoId", Name = "GetRandomVideoId")]
        public IActionResult GetRandomVideoId()
        {
            try
            {
                var randomVideo = _dbContext.Set<Reel>().OrderBy(r => Guid.NewGuid()).FirstOrDefault();

                if (randomVideo == null)
                {
                    return NotFound("No videos found in the database.");
                }

                return Ok(randomVideo.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting random video ID: {ex.Message}");
                return StatusCode(500, "An error occurred while getting the random video ID.");
            }
        }

        [HttpGet("Test/{videoId}", Name = "GetReelTest")]
        public IActionResult GetReelTest(int videoId)
        {
            var videoPath = _reelService.GetReelPathTest(videoId);

            if (string.IsNullOrEmpty(videoPath))
            {
                return NotFound();
            }

            try
            {
                var videoStream = new FileStream(videoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(videoStream, "video/mp4", enableRangeProcessing: true);
            }
            catch (IOException)
            {
                return StatusCode(500, "An error occurred while attempting to read the video file.");
            }
        }

        //[HttpPost("upload")]
        //public IActionResult UploadReel(IFormFile videoFile)
        //{

        //    if (videoFile == null || videoFile.Length == 0)
        //    {
        //        return BadRequest("No video file provided");
        //    }

        //    // Validate other properties in the ReelDto if needed

        //    try
        //    {
        //        // Save the video
        //        ReelDto reelDto = _reelService.SaveVideo(videoFile);

        //        // Get the video duration using the new method
        //        var videoPath = _reelService.GetReelPath(reelDto.Id);

        //        // Generate a thumbnail after successful video upload
        //        var outputPath = Path.Combine(_env.WebRootPath, "Thumbnails");
        //        var thumbnailPath = _reelService.ExtractThumbnailAsync(videoPath, outputPath).Result;

        //        if (string.IsNullOrEmpty(thumbnailPath))
        //        {
        //            // Handle the case where thumbnail generation fails (log or handle as needed)
        //            _logger.LogWarning("Thumbnail generation failed after video upload");
        //        }

        //        return Ok(new { Message = "Video uploaded and saved successfully"});
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error uploading video: {ex.Message}");
        //        return StatusCode(500, "An error occurred while uploading the video");
        //    }
        //}

        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultipleReels(List<IFormFile> videoFiles)
        {
            // ...omitting initial checks and try-catch for brevity

            List<string> videoPaths = new List<string>();
            List<ReelDto> reelDtos = new List<ReelDto>();

            foreach (var videoFile in videoFiles)
            {
                if (videoFile == null || videoFile.Length == 0) continue;

                var reelId = Guid.NewGuid();
                var videoPath = await _reelService.SaveVideo(videoFile, reelId);
                videoPaths.Add(videoPath);

                var reelDto = new ReelDto
                {
                    Id = reelId,
                    AudioTranscription = null,
                    Duration = null // Will be set after getting duration
                };
                reelDtos.Add(reelDto);
            }

            // Ensure all files are saved before proceeding
            for (int i = 0; i < videoPaths.Count; i++)
            {
                var reelDto = reelDtos[i];
                var videoPath = videoPaths[i];

                // Set the duration of the video
                reelDto.Duration = await _reelService.GetVideoDurationAsync(videoPath);

                // Save the Reel entity to the database
                var newReel = _mapper.Map<Reel>(reelDto);
                _dbContext.Reels.Add(newReel);
            }
            _dbContext.SaveChanges();

            // Now generate thumbnails
            var thumbnailTasks = reelDtos.Select(reelDto =>
                _reelService.ExtractThumbnailAsync(_reelService.GetReelPath(reelDto.Id), Path.Combine(_env.WebRootPath, "Thumbnails"))
            ).ToList();

            await Task.WhenAll(thumbnailTasks);

            return Ok(new { Message = "Videos uploaded and saved successfully", Reels = reelDtos });
        }



        [HttpDelete("{videoId}")]
        public IActionResult DeleteReel(Guid videoId)
        {
            try
            {
                // Delete video file
                var videoPath = _reelService.GetReelPath(videoId);
                if (System.IO.File.Exists(videoPath))
                {
                    System.IO.File.Delete(videoPath);
                }

                // Delete thumbnail file
                var outputPath = Path.Combine(_env.WebRootPath, "Thumbnails");
                var thumbnailFileName = $"{videoId}.jpg";
                var thumbnailPath = Path.Combine(outputPath, thumbnailFileName);
                if (System.IO.File.Exists(thumbnailPath))
                {
                    System.IO.File.Delete(thumbnailPath);
                }

                // Delete database entry
                var reel = _dbContext.Set<Reel>().Find(videoId);
                if (reel != null)
                {
                    _dbContext.Set<Reel>().Remove(reel);
                    _dbContext.SaveChanges();
                }

                return Ok("Reel deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting reel: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the reel");
            }
        }



    }
}
