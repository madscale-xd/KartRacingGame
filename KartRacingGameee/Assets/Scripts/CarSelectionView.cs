using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSelectionView : View
{
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _creditsButton;

    [SerializeField] private InstantiatePlayer ins;


    public override void Initialize()
    {

    }

        void Start()
    {

    }

        void Update()
    {

    }

    public void SpawnRolly(){
        ins.SpawnPlayer(0, "Player");
        ViewManager.Show<UIView>();
        Time.timeScale = 1f;
    }

    public void SpawnFleet(){
        ins.SpawnPlayer(1, "Player");
         ViewManager.Show<UIView>();
         Time.timeScale = 1f;
    }

    public void SpawnStewie(){
        ins.SpawnPlayer(2, "Player");
         ViewManager.Show<UIView>();
         Time.timeScale = 1f;
    }
}
