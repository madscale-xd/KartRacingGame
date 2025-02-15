using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSelectionView : View
{
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _creditsButton;

    [SerializeField] private GameObject rollyModel;
    [SerializeField] private GameObject fleetModel;
    [SerializeField] private GameObject stewieModel;
    [SerializeField] private GameObject selectionBG1;
    [SerializeField] private GameObject selectionBG2;

    [SerializeField] private InstantiatePlayer ins;

    public override void Initialize()
    {

    }

    void Start()
    {
        Cursor.visible = true;
    }

    void Update()
    {

    }

    public void SpawnRolly()
    {
        ins.SpawnPlayer(0, "Player");
        ViewManager.Show<UIView>();
        Time.timeScale = 1f;
        HideModels();
    }

    public void SpawnFleet()
    {
        ins.SpawnPlayer(1, "Player");
        ViewManager.Show<UIView>();
        Time.timeScale = 1f;
        HideModels();
    }

    public void SpawnStewie()
    {
        ins.SpawnPlayer(2, "Player");
        ViewManager.Show<UIView>();
        Time.timeScale = 1f;
        HideModels();
    }

    private void HideModels()
    {
        Cursor.visible = false;
        rollyModel.SetActive(false);
        fleetModel.SetActive(false);
        stewieModel.SetActive(false);
        selectionBG1.SetActive(false);
        selectionBG2.SetActive(false);
    }
}
