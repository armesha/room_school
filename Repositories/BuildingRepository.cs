// Repositories/BuildingRepository.cs
using RoomReservationSystem.Data;
using RoomReservationSystem.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;

namespace RoomReservationSystem.Repositories
{
    public class BuildingRepository : IBuildingRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BuildingRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Building> GetAllBuildings()
        {
            var buildings = new List<Building>();
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                string query = "SELECT building_id, building_name, address, description, image FROM buildings";

                using (var command = new OracleCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var building = new Building
                            {
                                BuildingId = reader.GetInt32(0),
                                BuildingName = reader.GetString(1),
                                Address = reader.GetString(2),
                                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Image = reader.IsDBNull(4) ? null : (byte[])reader["image"]
                            };
                            buildings.Add(building);
                        }
                    }
                }
            }
            return buildings;
        }

        public Building GetBuildingById(int buildingId)
        {
            Building building = null;
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                string query = "SELECT building_id, building_name, address, description, image FROM buildings WHERE building_id = :id";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("id", buildingId));

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            building = new Building
                            {
                                BuildingId = reader.GetInt32(0),
                                BuildingName = reader.GetString(1),
                                Address = reader.GetString(2),
                                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Image = reader.IsDBNull(4) ? null : (byte[])reader["image"]
                            };
                        }
                    }
                }
            }
            return building;
        }

        public void AddBuilding(Building building)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                string query = @"INSERT INTO buildings (building_id, building_name, address, description, image)
                                 VALUES (seq_buildings.NEXTVAL, :buildingName, :address, :description, :image)
                                 RETURNING building_id INTO :buildingId";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("buildingName", building.BuildingName));
                    command.Parameters.Add(new OracleParameter("address", building.Address));
                    command.Parameters.Add(new OracleParameter("description", (object)building.Description ?? DBNull.Value));
                    command.Parameters.Add(new OracleParameter("image", building.Image != null ? (object)building.Image : DBNull.Value));

                    var buildingIdParam = new OracleParameter("buildingId", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(buildingIdParam);

                    command.ExecuteNonQuery();

                    // Retrieve the generated BuildingId
                    building.BuildingId = Convert.ToInt32(buildingIdParam.Value.ToString());
                }
            }
        }

        public void UpdateBuilding(Building building)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                string query = @"UPDATE buildings
                                 SET building_name = :buildingName,
                                     address = :address,
                                     description = :description,
                                     image = :image
                                 WHERE building_id = :buildingId";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("buildingName", building.BuildingName));
                    command.Parameters.Add(new OracleParameter("address", building.Address));
                    command.Parameters.Add(new OracleParameter("description", (object)building.Description ?? DBNull.Value));
                    command.Parameters.Add(new OracleParameter("image", building.Image != null ? (object)building.Image : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("buildingId", building.BuildingId));

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteBuilding(int buildingId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                string query = "DELETE FROM buildings WHERE building_id = :buildingId";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("buildingId", buildingId));
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
