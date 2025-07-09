using MasterWord.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ResDb;
using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace MasterWord.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterWord : ControllerBase
    {
        private readonly DatabaseContext _dbContext;
        private readonly DropboxService _dropboxService;

        public MasterWord(DatabaseContext dbContext, DropboxService dropboxService)
        {
            _dbContext = dbContext;
            _dropboxService = dropboxService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var allWords = await _dbContext.MasterProjectReservedWord_BK
                                           .Where(w => w.IsDeleted == false)
                                           .OrderByDescending(w => w.UpdateDate ?? w.CreateDate)
                                           .Select(w => new
                                           {
                                               w.Id,
                                               WordName = w.WordName ?? string.Empty,
                                               w.CreateDate,
                                               CreateBy = w.CreateBy ?? string.Empty,
                                               w.UpdateDate,
                                               UpdateBy = w.UpdateBy ?? string.Empty,
                                               w.IsDeleted,
                                               w.IsActive,
                                               FilePath = w.FilePath ?? string.Empty
                                           })
                                           .ToListAsync();
            List<object> _allWords = new List<object>();
            int Sequence = 0;
            foreach (var item in allWords)
            {
                _allWords.Add(new
                {
                    Id = item.Id,
                    WordName = item.WordName,
                    CreateDate = item.CreateDate,
                    CreateBy = item.CreateBy,
                    UpdateDate = item.UpdateDate,
                    UpdateBy = item.UpdateBy,
                    IsDeleted = item.IsDeleted,
                    IsActive = item.IsActive,
                    Sequence = Sequence + 1,
                    FilePath = item.FilePath,
                });
                Sequence = Sequence + 1;
            }

            if (_allWords.Any())
            {
                return Ok(_allWords);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet("all-admin")]
        public async Task<IActionResult> GetAllAdmin()
        {
            var allWords = await _dbContext.MasterProjectReservedWord_BK
                                           .OrderByDescending(w => w.UpdateDate ?? w.CreateDate)
                                           .Select(w => new
                                           {
                                               w.Id,
                                               WordName = w.WordName ?? string.Empty,
                                               w.CreateDate,
                                               CreateBy = w.CreateBy ?? string.Empty,
                                               w.UpdateDate,
                                               UpdateBy = w.UpdateBy ?? string.Empty,
                                               w.IsDeleted,
                                               w.IsActive,
                                               FilePath = w.FilePath ?? string.Empty
                                           })
                                           .ToListAsync();
            List<object> _allWords = new List<object>();
            int Sequence = 0;
            foreach (var item in allWords)
            {
                _allWords.Add(new
                {
                    Id = item.Id,
                    WordName = item.WordName,
                    CreateDate = item.CreateDate,
                    CreateBy = item.CreateBy,
                    UpdateDate = item.UpdateDate,
                    UpdateBy = item.UpdateBy,
                    IsDeleted = item.IsDeleted,
                    IsActive = item.IsActive,
                    Sequence = Sequence + 1,
                    FilePath = item.FilePath,
                });
                Sequence = Sequence + 1;
            }

            if (_allWords.Any())
            {
                return Ok(_allWords);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetId(string id)
        {
            var word = await _dbContext.MasterProjectReservedWord_BK
                                       .Where(w => w.Id == id)
                                       .Select(w => new
                                       {
                                           w.Id,
                                           w.WordName,
                                           w.CreateDate,
                                           w.CreateBy,
                                           w.UpdateDate,
                                           w.UpdateBy,
                                           w.IsDeleted,
                                           w.IsActive,
                                           w.FilePath
                                       })
                                       .FirstOrDefaultAsync();
            if (word == null)
            {
                return NotFound();
            }
            var result = new
            {
                word.Id,
                word.WordName,
                word.CreateDate,
                word.CreateBy,
                word.UpdateDate,
                word.UpdateBy,
                word.IsDeleted,
                word.IsActive,
                FilePath = word.FilePath ?? string.Empty,
            };
            return Ok(result);
        }


        [HttpPost("GetMulti")]
        public async Task<List<MasterProjectReservedWord_BKReq>> GetMulti([FromBody] List<string> ids)
        {
            var words = await _dbContext.MasterProjectReservedWord_BK
                .Where(w => ids.Contains(w.Id))
                .Select(w => new MasterProjectReservedWord_BKReq
                {
                    Id = w.Id,
                    WordName = w.WordName,
                    IsActive = w.IsActive,
                    IsDeleted = w.IsDeleted
                })
                .ToListAsync();

            return words ?? new List<MasterProjectReservedWord_BKReq>();
        }
   

        [HttpPost]
        public async Task<IActionResult> CreateWord([FromBody] MasterProjectReservedWord_BKReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isNameUsed = await _dbContext.MasterProjectReservedWord_BK
                .AnyAsync(w => w.WordName == req.WordName && (w.IsDeleted == null || w.IsDeleted == false));

            if (isNameUsed)
            {
                return Conflict(new { message = "ชื่อนี้มีอยู่ในระบบอยู่แล้ว : กรุณาใช้ชื่ออื่น" });
            }

            var newWord = new MasterProjectReservedWord_BK
            {
                Id = Guid.NewGuid().ToString(),
                WordName = req.WordName,
                CreateDate = DateTime.UtcNow,
                CreateBy = "System",
                UpdateDate = null,
                UpdateBy = "System",
                IsDeleted = req.IsDeleted ?? false,
                IsActive = req.IsActive ?? true,
                FilePath = string.Empty
            };

            _dbContext.MasterProjectReservedWord_BK.Add(newWord);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { id = newWord.Id }, newWord);
        }

        [HttpPut("PutWords")]
        public async Task<IActionResult> PutWords([FromBody] List<MasterProjectReservedWord_BKReq> list)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { error = string.Join("; ", errors) });
            }

            foreach (var req in list)
            {
                if (string.IsNullOrWhiteSpace(req.WordName))
                {
                    return BadRequest(new { error = "Word Name cannot be empty." });
                }

                bool isNameUsed = await _dbContext.MasterProjectReservedWord_BK
                    .AnyAsync(w => w.WordName == req.WordName && w.Id != req.Id && (w.IsDeleted == null || w.IsDeleted == false));

                if (isNameUsed)
                {
                    return Conflict(new { error = $"ชื่อ {req.WordName} มีอยู่ในระบบอยู่แล้ว : กรุณาใช้ชื่ออื่น" });
                }

                var wordToUpdate = await _dbContext.MasterProjectReservedWord_BK
                    .FirstOrDefaultAsync(w => w.Id == req.Id);

                if (wordToUpdate == null)
                {
                    return NotFound(new { error = $"ไม่พบคำ Id {req.Id}" });
                }

                wordToUpdate.WordName = req.WordName;
                if (req.IsActive.HasValue)
                    wordToUpdate.IsActive = req.IsActive.Value;

                if (req.IsDeleted.HasValue)
                    wordToUpdate.IsDeleted = req.IsDeleted.Value;

                wordToUpdate.UpdateDate = DateTime.UtcNow;
                wordToUpdate.UpdateBy = "System";

                _dbContext.MasterProjectReservedWord_BK.Update(wordToUpdate);
            }

            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Updated successfully", count = list.Count });
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteId(string id)
        {
            var word = await _dbContext.MasterProjectReservedWord_BK
                                       .FirstOrDefaultAsync(w => w.Id == id && (w.IsDeleted == null || w.IsDeleted == false));
            if (word == null)
            {
                return NotFound();
            }

            word.IsDeleted = true;
            word.UpdateDate = DateTime.UtcNow;
            word.UpdateBy = "System";

            _dbContext.MasterProjectReservedWord_BK.Update(word);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadRequest request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { error = "No file uploaded." });

            MasterProjectReservedWord_BK? fileRecord = null;
            if (!string.IsNullOrEmpty(request.Id))
            {
                fileRecord = await _dbContext.MasterProjectReservedWord_BK
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (fileRecord != null && !string.IsNullOrEmpty(fileRecord.FilePath))
                {
                    return Conflict(new
                    {
                        AlreadyExists = true,
                        FileUrl = fileRecord.FilePath,
                        Message = "Cannot upload: this record already has a file."
                    });
                }
            }

            var result = await _dropboxService.UploadFileAsync(request.File, cancellationToken);

            if (!result.Success)
                return StatusCode(500, new { error = result.ErrorMessage });

            if (fileRecord != null)
            {
                fileRecord.FilePath = result.FileUrl;
                fileRecord.UpdateDate = DateTime.UtcNow;
                fileRecord.UpdateBy = "System";
                _dbContext.Update(fileRecord);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Ok(new
            {
                FileName = result.FileName!,
                FileUrl = result.FileUrl!,
            });
        }

        [HttpDelete("delete-file/{id}")]
        public async Task<IActionResult> DeleteFile(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { error = "Id is required." });

            var fileRecord = await _dbContext.MasterProjectReservedWord_BK
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (fileRecord == null || string.IsNullOrEmpty(fileRecord.FilePath))
                return NotFound(new { error = "File not found for this record." });

            var deleteResult = await _dropboxService.DeleteFileAsync(fileRecord.FilePath, cancellationToken);
            if (!deleteResult.Success && (deleteResult.ErrorMessage == null || !deleteResult.ErrorMessage.Contains("not_found")))
                return StatusCode(500, new { error = deleteResult.ErrorMessage });

            fileRecord.FilePath = string.Empty;
            fileRecord.UpdateDate = DateTime.UtcNow;
            fileRecord.UpdateBy = "System";
            _dbContext.Update(fileRecord);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new
            {
                message = "ลบไฟล์สำเร็จ"
            });
        }

        [HttpPut("PutAll")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PutAll([FromBody] WordBulkUpdateMultipartDto form)
        {
            if (string.IsNullOrWhiteSpace(form.Data))
                return BadRequest(new { message = "ไม่พบข้อมูลสำหรับอัปเดต" });

            List<MasterProjectReservedWord_BKReq> items;
            try
            {
                items = JsonConvert.DeserializeObject<List<MasterProjectReservedWord_BKReq>>(form.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "รูปแบบข้อมูลไม่ถูกต้อง", error = ex.Message });
            }

            var errors = new List<object>();

            foreach (var req in items)
            {
                if (string.IsNullOrWhiteSpace(req.Id) || string.IsNullOrWhiteSpace(req.WordName))
                {
                    errors.Add(new { id = req.Id, error = "Id และ WordName ต้องไม่ว่าง" });
                    continue;
                }

                var word = await _dbContext.MasterProjectReservedWord_BK.FindAsync(req.Id);
                if (word == null)
                {
                    errors.Add(new { id = req.Id, error = "ไม่พบคำที่ต้องการแก้ไข" });
                    continue;
                }

                bool isNameUsed = await _dbContext.MasterProjectReservedWord_BK
                    .AnyAsync(w => w.WordName == req.WordName && w.Id != req.Id && (w.IsDeleted == null || w.IsDeleted == false));

                if (isNameUsed)
                {
                    errors.Add(new { id = req.Id, error = "ชื่อนี้มีอยู่ในระบบอยู่แล้ว" });
                    continue;
                }

                word.WordName = req.WordName;
                if (req.IsActive.HasValue) word.IsActive = req.IsActive.Value;
                if (req.IsDeleted.HasValue) word.IsDeleted = req.IsDeleted.Value;
                word.UpdateDate = DateTime.UtcNow;
                word.UpdateBy = "System";

                _dbContext.MasterProjectReservedWord_BK.Update(word);
            }

            await _dbContext.SaveChangesAsync();

            if (errors.Any())
                return BadRequest(new { message = "อัปเดตบางรายการไม่สำเร็จ", errors });

            return Ok(new { message = "อัปเดตทั้งหมดสำเร็จ" });
        }


    }
}
