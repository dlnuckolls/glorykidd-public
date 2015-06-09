using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{

	[Serializable]
	public class FacilityCollection : Dictionary<int, Data.FacilityDataRow>
	{
		private static string CACHE_KINDRED_FACILITY_COLLECTION = "KindredFacilityCollection";

		#region static methods

		public List<Data.FacilityDataRow> SortedList(FacilitySortField facilitySortField)
		{
			List<Data.FacilityDataRow> rtn = new List<Kindred.Knect.ITAT.Data.FacilityDataRow>(this.Values);
			switch (facilitySortField)
			{
				case FacilitySortField.FacilityId:
					rtn.Sort(Data.FacilityDataRow.CompareByFacId);
					break;
				case FacilitySortField.SapId:
					rtn.Sort(Data.FacilityDataRow.CompareBySapId);
					break;
				case FacilitySortField.FacilityName:
					rtn.Sort(Data.FacilityDataRow.CompareByName);
					break;
				default:
					break;
			}
			return rtn;
		}


		public static FacilityCollection GetAll(bool useCache)
		{
			FacilityCollection coll = null;
			//Try to get the collection of all Kindred facilities from the Cache.  If so, return it.
			if (useCache)
			{
				try 
				{
					coll = (FacilityCollection)System.Web.HttpContext.Current.Cache[CACHE_KINDRED_FACILITY_COLLECTION];
				}
				catch (Exception ex) 
				{
					throw new Exception(String.Format("Error retrieving the Kindred Facilities from the cache,   Error: {0}", ex.Message));
				}
			}
			if (coll == null)
			{
				//Create the facility collection, and add it to the Cache.
				coll = GetAllFacilityCollection();
				if (useCache)
				{
					try
					{
						System.Web.HttpContext.Current.Cache.Insert(CACHE_KINDRED_FACILITY_COLLECTION, coll, null, DateTime.Now.AddHours(4), System.Web.Caching.Cache.NoSlidingExpiration);
					}
					catch (Exception ex) 
					{ 
						throw new Exception(String.Format("Error updating the cache containing Kindred Facilities,   Error: {0}", ex.Message)); 
					}
				}
			}
			return coll;
		}

		private static FacilityCollection GetAllFacilityCollection()
		{
			DataTable allFacs = Data.Facility.AllFacilities(true).Tables[0];
			FacilityCollection coll = new FacilityCollection();
			foreach (DataRow row in allFacs.Rows)
			{
                string facility_name = string.Empty;
                try
                {
                    facility_name = row["facility_name"].ToString();
                }
                catch
                {
                    throw new Exception("Unable to parse facility_name");
                }

                string facility_alias_id = string.Empty;
                try
                {
                    facility_alias_id = row["facility_alias_id"].ToString();
                }
                catch
                {
                    throw new Exception(string.Format("Unable to parse facility_alias_id for facility_name '{0}'", facility_name));
                }

                string facilityID = string.Format("facility_name '{0}', facility_alias '{1}'", facility_name, facility_alias_id);

                int facId;
                if (!int.TryParse(row["facility_id"].ToString(), out facId))
                    throw new Exception(string.Format("Unable to parse facility_id for {0}", facilityID));

                int facility_type_code;
                if (!int.TryParse(row["facility_type_code"].ToString(), out facility_type_code))
                    throw new Exception(string.Format("Unable to parse facility_type_code for {0}", facilityID));

                if (row.IsNull("status"))
                    throw new Exception(string.Format("Status not defined for {0}", facilityID));

                int facility_status;
                if (!int.TryParse(row["status"].ToString(), out facility_status))
                    throw new Exception(string.Format("Unable to parse status for {0}", facilityID));

                int orgLevel;
                if (!int.TryParse(row["orgLevel"].ToString(), out orgLevel))
                    throw new Exception(string.Format("Unable to parse orgLevel for facility {0}", facilityID));

                coll.Add(facId, new Kindred.Knect.ITAT.Data.FacilityDataRow(
							facId,
							row["facility_name"].ToString(),
							row["facility_alias_id"].ToString(),
							"|" + row["opPath"].ToString() + "|",
                            facility_type_code,
							row["facility_type_name"].ToString(),
                            facility_status,
                            orgLevel,
							string.Empty,
							string.Empty,
							string.Empty,
							string.Empty,
							string.Empty,
							string.Empty,
							string.Empty,
							string.Empty,
							string.Empty,
							string.Empty
						));
			}

			Data.FacilityDataRow facDataRow;
			DataTable allFacsInfo = Data.Facility.GetFacilityInfoAll().Tables[0];
			foreach (DataRow row in allFacsInfo.Rows)
			{
				int facId = int.Parse(row["FacilityID"].ToString());
				if (coll.TryGetValue(facId, out facDataRow))
				{
					facDataRow.Address = (row.IsNull("Address") ? string.Empty : row["Address"].ToString());
					facDataRow.City = (row.IsNull("City") ? string.Empty : row["City"].ToString());
					facDataRow.County = (row.IsNull("County") ? string.Empty : row["County"].ToString());
					facDataRow.State = (row.IsNull("StateName") ? string.Empty : row["StateName"].ToString());
					facDataRow.StateCode = (row.IsNull("StateCode") ? string.Empty : row["StateCode"].ToString());
					facDataRow.Zip = (row.IsNull("Zip") ? string.Empty : row["Zip"].ToString());
					facDataRow.AreaCode = (row.IsNull("AreaCode") ? string.Empty : row["AreaCode"].ToString());
					facDataRow.Phone = (row.IsNull("Phone") ? string.Empty : row["Phone"].ToString());
					facDataRow.Fax = (row.IsNull("Fax") ? string.Empty : row["Fax"].ToString());
					facDataRow.LegalEntityName = (row.IsNull("LegalEntityName") ? string.Empty : row["LegalEntityName"].ToString());
				}
			}
			return coll;
		}



		public static FacilityCollection GetAll()
		{
			return GetAll(System.Web.HttpContext.Current != null);
		}


		public static FacilityCollection FacilityList(int facilityId, bool includeChildren)
		{
			List<int> facilityIds = new List<int>(1);
			facilityIds.Add(facilityId);
			return  FacilityCollection.FacilityList(facilityIds, includeChildren);
		}

		public static FacilityCollection FacilityList(List<int> facilityIds, bool includeChildren)
		{
			List<int> usedFacilities = new List<int>();
			FacilityCollection rtn = new FacilityCollection();
			List<string> stringFacList = new List<string>(facilityIds.Count);
			for (int i = 0; i < facilityIds.Count; i++)
				stringFacList.Add("|" + facilityIds[i].ToString() + "|");
			FacilityCollection allFacilities = FacilityCollection.GetAll();

			//special case:  if facilityIds contains Corporate and includeChildren == true, then just return allFacilities
			if (facilityIds.Contains(Data.Facility.CorporateFacilityId) && includeChildren)
			{
				return allFacilities;
			}
			else
			{
				foreach (int facId in facilityIds)
				{
					if (allFacilities != null)
					{
						foreach (Data.FacilityDataRow row in allFacilities.Values)
						{
							for (int i = 0; i < facilityIds.Count; i++)
							{
								if ((row.FacilityId == facilityIds[i]) || ((includeChildren) && (row.FacilityOpPath.IndexOf(stringFacList[i]) > -1)))
								{
									int facID = row.FacilityId;
									if (!usedFacilities.Contains(facID))
									{
										usedFacilities.Add(facID);   //"facility_id" column
										rtn.Add(facID, row);
									}
								}
							}
						}
					}
				}
				return rtn;
			}
		}


		/// <summary>
		/// Returns the subset of "unfilteredList" that match the criteria specified in facilityTerm
		/// </summary>
		/// <param name="unfilteredList"></param>
		/// <param name="facilityTerm"></param>
		/// <returns></returns>
		public static FacilityCollection FilteredFacilityList(FacilityCollection unfilteredFacilityList, FacilityTerm facilityTerm)
		{
			FacilityCollection rtn = new Business.FacilityCollection();
			foreach (Data.FacilityDataRow facDataRow in unfilteredFacilityList.Values)
			{
				//if the FacilityDataRow's FacilityStatus and FacilityType meet the criteria in facilityTerm, copy the item to the "rtn" list
				if ((facilityTerm.FacilityStatus == FacilityStatusType.All) || ((int)facilityTerm.FacilityStatus == facDataRow.FacilityStatusId))
					if (facilityTerm.FacilityTypes.Contains(facDataRow.FacilityTypeId))
						rtn.Add(facDataRow.FacilityId, facDataRow);
			}
			return rtn;
		}


		/// <summary>
		/// Takes a list of FacilityIDs, and returns a list of FacilityIDs for all facilities that are ancestors of any facility in the input list (including those facilities)
		/// </summary>
		/// <param name="facIDs">A list of facility IDs</param>
		/// <returns>A list of facility IDs who are "ancestors" of the input list of facilities (including those facilities)</returns>
		public static List<int> FacilityAncestry(List<int> owningFacIDs, List<int> absLevels, List<int> relLevels)
		{
			List<int> rtn = new List<int>();
			if (owningFacIDs != null)
			{
				//convert absLevels (or all levels if restrictions not enforced) to database levels
				List<int> databaseLevels = Role.GetAllDatabaseLevels();
				if ((absLevels != null) || (relLevels != null))
					databaseLevels = Role.AbsoluteToDatabaseLevel(absLevels);
				FacilityCollection allFacilities = FacilityCollection.GetAll();

				foreach (int owningFacID in owningFacIDs)
				{
					Data.FacilityDataRow facDataRow = allFacilities[owningFacID];
					if (facDataRow != null)
					{
						//convert relLevels to database levels and combine with databaseLevels
						List<int> targetDatabaseLevels = Role.CombineList(databaseLevels, Role.RelativeToDatabaseLevel(allFacilities[owningFacID].Level, relLevels));

						string[] opPathSegments = facDataRow.FacilityOpPath.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string opPathSegment in opPathSegments)
						{
							int newFacID = int.Parse(opPathSegment);
							if (!rtn.Contains(newFacID))
								if (targetDatabaseLevels.Contains(allFacilities[newFacID].Level))
									rtn.Add(newFacID);
						}
					}
				}
			}
			return rtn;
		}



		#endregion


		#region  constructors (pass-through to List<T> constructors)

		public FacilityCollection()
			: base()
		{
		}


		public FacilityCollection(IDictionary<int, Data.FacilityDataRow> collection) 
			: base(collection) 
		{
		}

		#endregion


		#region  Public methods

		#endregion


	}
}
