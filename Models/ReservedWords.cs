using System;

namespace ResDb;

public class MasterProjectReservedWord
{
    public required string Id { get; set; }
    public required string WordName { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? CreateBy { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? UpdateBy { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
   
}
public class MasterProjectReservedWordRespond
{
    public required string Id { get; set; }
    public required string WordName { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? CreateBy { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? UpdateBy { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public int Sequence { get; set; }
}

public class MasterProjectReservedWordReq
{
    public required string Id { get; set; }

    public required string WordName { get; set; }

    public bool? IsActive { get; set; }

}

    public class FileUploadRequest
{
    public required IFormFile File { get; set; }
}

public class FileUploadResponse
{
    public required string FileName { get; set; }
    public required string FileUrl { get; set; }
}
public class FileUploadResult
{
    public bool Success { get; set; }
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }
    public string? ErrorMessage { get; set; }
}
