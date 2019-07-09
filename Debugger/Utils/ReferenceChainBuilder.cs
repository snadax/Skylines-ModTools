using ModTools.Explorer;

namespace ModTools.Utils
{
    internal static class ReferenceChainBuilder
    {
        public static ReferenceChain ForEditPrefabInfo()
        {
            return new ReferenceChain()
                .Add(ToolManager.instance.gameObject)
                .Add(ToolManager.instance)
                .Add(typeof(ToolManager).GetField("m_properties"))
                .Add(typeof(ToolController).GetField("m_editPrefabInfo"));
        }

        public static ReferenceChain ForBuilding(ushort buildingId)
        {
            return new ReferenceChain()
                .Add(BuildingManager.instance.gameObject)
                .Add(BuildingManager.instance)
                .Add(typeof(BuildingManager).GetField("m_buildings"))
                .Add(typeof(Array16<Building>).GetField("m_buffer"))
                .Add(buildingId);
        }

        public static ReferenceChain ForVehicle(ushort vehicleId)
        {
            return new ReferenceChain()
                .Add(VehicleManager.instance.gameObject)
                .Add(VehicleManager.instance)
                .Add(typeof(VehicleManager).GetField("m_vehicles"))
                .Add(typeof(Array16<Vehicle>).GetField("m_buffer"))
                .Add(vehicleId);
        }

        public static ReferenceChain ForParkedVehicle(ushort parkedVehicleId)
        {
            return new ReferenceChain()
                .Add(VehicleManager.instance.gameObject)
                .Add(VehicleManager.instance)
                .Add(typeof(VehicleManager).GetField("m_parkedVehicles"))
                .Add(typeof(Array16<VehicleParked>).GetField("m_buffer"))
                .Add(parkedVehicleId);
        }

        public static ReferenceChain ForCitizenInstance(ushort citizenInstanceId)
        {
            return new ReferenceChain()
                .Add(CitizenManager.instance.gameObject)
                .Add(CitizenManager.instance)
                .Add(typeof(CitizenManager).GetField("m_instances"))
                .Add(typeof(Array16<CitizenInstance>).GetField("m_buffer"))
                .Add(citizenInstanceId);
        }

        public static ReferenceChain ForCitizen(uint citizenId)
        {
            return new ReferenceChain()
                .Add(CitizenManager.instance.gameObject)
                .Add(CitizenManager.instance)
                .Add(typeof(CitizenManager).GetField("m_citizens"))
                .Add(typeof(Array32<Citizen>).GetField("m_buffer"))
                .Add(citizenId);
        }

        public static ReferenceChain ForCitizenUnit(uint citizenUnitId)
        {
            return new ReferenceChain()
                .Add(CitizenManager.instance.gameObject)
                .Add(CitizenManager.instance)
                .Add(typeof(CitizenManager).GetField("m_units"))
                .Add(typeof(Array32<CitizenUnit>).GetField("m_buffer"))
                .Add(citizenUnitId);
        }

        public static ReferenceChain ForTransportLine(ushort transportLineId)
        {
            return new ReferenceChain()
                .Add(TransportManager.instance.gameObject)
                .Add(TransportManager.instance)
                .Add(typeof(TransportManager).GetField("m_lines"))
                .Add(typeof(Array16<TransportLine>).GetField("m_buffer"))
                .Add(transportLineId);
        }

        public static ReferenceChain ForPathUnit(uint pathUnitId)
        {
            return new ReferenceChain()
                .Add(PathManager.instance.gameObject)
                .Add(PathManager.instance)
                .Add(typeof(PathManager).GetField("m_pathUnits"))
                .Add(typeof(Array32<PathUnit>).GetField("m_buffer"))
                .Add(pathUnitId);
        }

        public static ReferenceChain ForNode(ushort nodeId)
        {
            return new ReferenceChain()
                .Add(NetManager.instance.gameObject)
                .Add(NetManager.instance)
                .Add(typeof(NetManager).GetField("m_nodes"))
                .Add(typeof(Array16<NetNode>).GetField("m_buffer"))
                .Add(nodeId);
        }
        
        public static ReferenceChain ForSegment(ushort segmentId)
        {
            return new ReferenceChain()
                .Add(NetManager.instance.gameObject)
                .Add(NetManager.instance)
                .Add(typeof(NetManager).GetField("m_segments"))
                .Add(typeof(Array16<NetSegment>).GetField("m_buffer"))
                .Add(segmentId);
        }
        
        public static ReferenceChain ForLane(uint laneId)
        {
            return new ReferenceChain()
                .Add(NetManager.instance.gameObject)
                .Add(NetManager.instance)
                .Add(typeof(NetManager).GetField("m_lanes"))
                .Add(typeof(Array32<NetLane>).GetField("m_buffer"))
                .Add(laneId);
        }
        
        public static ReferenceChain ForDistrict(byte districtId)
        {
            return new ReferenceChain()
                .Add(DistrictManager.instance.gameObject)
                .Add(DistrictManager.instance)
                .Add(typeof(DistrictManager).GetField("m_districts"))
                .Add(typeof(Array8<District>).GetField("m_buffer"))
                .Add(districtId);
        }
        
        public static ReferenceChain ForPark(byte parkId)
        {
            return new ReferenceChain()
                .Add(DistrictManager.instance.gameObject)
                .Add(DistrictManager.instance)
                .Add(typeof(DistrictManager).GetField("m_parks"))
                .Add(typeof(Array8<DistrictPark>).GetField("m_buffer"))
                .Add(parkId);
        }
    }
}