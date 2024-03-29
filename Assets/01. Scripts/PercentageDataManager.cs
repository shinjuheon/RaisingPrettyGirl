using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public class PercentageDataManager : MonoBehaviour
    {
        private List<Dictionary<string, object>> data_skin;
        public  List<Dictionary<string, object>> Data_skin => data_skin;

        private List<Dictionary<string, object>> data_pet;
        public List<Dictionary<string, object>> Data_pet => data_pet;
        private void Init()
        {
            data_skin = CSVReader.Read("Skin_Percentage");
            data_pet = CSVReader.Read("Pet_Percentage");
        }

        private void Awake()
        {
            Init();
        }
    }
}
