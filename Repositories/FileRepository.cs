// Repositories/FileRepository.cs
using Oracle.ManagedDataAccess.Client;
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;

// Alias for the custom File model
using FileModel = RoomReservationSystem.Models.File;

namespace RoomReservationSystem.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FileRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<FileModel> GetAllFilesForUser(int userId)
        {
            var files = new List<FileModel>();
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT file_id, uploaded_by, file_name, file_type, file_extension, upload_date, modification_date, operation, file_content 
                                    FROM files 
                                    WHERE uploaded_by = :user_id";
            command.Parameters.Add(new OracleParameter("user_id", OracleDbType.Int32) { Value = userId });

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                files.Add(new FileModel
                {
                    FileId = Convert.ToInt32(reader["file_id"]),
                    UploadedBy = Convert.ToInt32(reader["uploaded_by"]),
                    FileName = reader["file_name"].ToString(),
                    FileType = reader["file_type"].ToString(),
                    FileExtension = reader["file_extension"].ToString(),
                    UploadDate = Convert.ToDateTime(reader["upload_date"]),
                    ModificationDate = reader["modification_date"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["modification_date"]) : null,
                    Operation = reader["operation"].ToString(),
                    FileContent = (byte[])reader["file_content"]
                });
            }
            return files;
        }

        public FileModel GetFileById(int fileId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT file_id, uploaded_by, file_name, file_type, file_extension, upload_date, modification_date, operation, file_content 
                                    FROM files WHERE file_id = :file_id";
            command.Parameters.Add(new OracleParameter("file_id", OracleDbType.Int32) { Value = fileId });

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new FileModel
                {
                    FileId = Convert.ToInt32(reader["file_id"]),
                    UploadedBy = Convert.ToInt32(reader["uploaded_by"]),
                    FileName = reader["file_name"].ToString(),
                    FileType = reader["file_type"].ToString(),
                    FileExtension = reader["file_extension"].ToString(),
                    UploadDate = Convert.ToDateTime(reader["upload_date"]),
                    ModificationDate = reader["modification_date"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["modification_date"]) : null,
                    Operation = reader["operation"].ToString(),
                    FileContent = (byte[])reader["file_content"]
                };
            }
            return null;
        }

        public void AddFile(FileModel file)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO files 
                                    (file_id, uploaded_by, file_name, file_type, file_extension, upload_date, modification_date, operation, file_content) 
                                    VALUES 
                                    (seq_files.NEXTVAL, :uploaded_by, :file_name, :file_type, :file_extension, :upload_date, :modification_date, :operation, :file_content)";
            command.Parameters.Add(new OracleParameter("uploaded_by", OracleDbType.Int32) { Value = file.UploadedBy });
            command.Parameters.Add(new OracleParameter("file_name", OracleDbType.Varchar2) { Value = file.FileName });
            command.Parameters.Add(new OracleParameter("file_type", OracleDbType.Varchar2) { Value = file.FileType });
            command.Parameters.Add(new OracleParameter("file_extension", OracleDbType.Varchar2) { Value = file.FileExtension });
            command.Parameters.Add(new OracleParameter("upload_date", OracleDbType.Date) { Value = file.UploadDate });
            command.Parameters.Add(new OracleParameter("modification_date", OracleDbType.Date) { Value = (object)file.ModificationDate ?? DBNull.Value });
            command.Parameters.Add(new OracleParameter("operation", OracleDbType.Varchar2) { Value = file.Operation });
            command.Parameters.Add(new OracleParameter("file_content", OracleDbType.Blob) { Value = file.FileContent });

            command.ExecuteNonQuery();
        }

        public void DeleteFile(int fileId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM files WHERE file_id = :file_id";
            command.Parameters.Add(new OracleParameter("file_id", OracleDbType.Int32) { Value = fileId });

            command.ExecuteNonQuery();
        }
    }
}
