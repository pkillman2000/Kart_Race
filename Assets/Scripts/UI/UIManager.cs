using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Vehicle
{
    public string Name = "";
    public int CurrentLap = 0;
    public float CurrentLapPercentage = 0f;
    public float RacePercentage = 0f;
}

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Gauge _gauge;

    [SerializeField]
    private TMP_Text _numberOfLapsText;
    [SerializeField]
    private TMP_Text _lapPercentage;

    public List<Vehicle> Vehicles;
    public List<Vehicle> RaceResults;

    public GameObject leaderboardItemPrefab;
    public Transform leaderboardContainer;


    void Start()
    {
        StartCoroutine(UpdateLeaderboard());
    }


    void Update()
    {

    }

    public void SetCurrentGearName(string currentGearName)
    {
        _gauge.SetGear(currentGearName);
    }

    public void SetSpeed(float speed, string speedText)
    {
        _gauge.SetSpeedometerNeedle(speed);
        _gauge.SetSpeed(speedText);
    }

    public void SetTachometer(float value)
    {
        _gauge.SetTachometer(value);
    }

    public void UpdateVehiclelist()
    {
        Vehicles.Clear();
        RaceResults.Clear();
        var vehicles = FindObjectsByType<CarController>(FindObjectsSortMode.None);

        foreach (var vehicle in vehicles)
        {
            Vehicle newItem = new Vehicle();
            newItem.Name = vehicle._carName;
            newItem.CurrentLap = vehicle._currentLap;
            newItem.CurrentLapPercentage = vehicle._currentLapPercentage;
            newItem.RacePercentage = vehicle._racePercentage;
            Vehicles.Add(newItem);
        }

        RaceResults = Vehicles.OrderByDescending(r => r.RacePercentage).ToList();
    }

    IEnumerator UpdateLeaderboard()
    {
        while (true)
        {
            UpdateVehiclelist();

            // Update Leaderboard UI here using the updated Vehicles list
            foreach (Transform child in leaderboardContainer) // Clear existing rows
            {
                Destroy(child.gameObject);
            }

            int rank = 1;
            foreach (var racer in RaceResults) // Instantiate new rows
            {
                GameObject row = Instantiate(leaderboardItemPrefab, leaderboardContainer);

                // Assuming your Row Prefab has a script attached to update its text
                LeaderboardItem leaderboardItem = row.GetComponent<LeaderboardItem>();
                leaderboardItem.UpdateItem(rank, racer.Name);

                rank++;
            }

            yield return new WaitForSeconds(1.0f); // Update every second
        }
    }

    internal void SetRaceProgress(int currentLap, int numberOfLaps, float lapPercentage)
    {
        _numberOfLapsText.text = $"{currentLap}/{numberOfLaps}";
        _lapPercentage.text = lapPercentage.ToString("F0") + "%";
    }
}