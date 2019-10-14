using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class Parabola : MonoBehaviour
{
   public GameObject BallPrefab;
   public GameObject Prefab;
   public GameObject[] ListObjects;
   public float Speed;
   
   public float A;
   public float B;
   public float C;
   public float L;
   [SerializeField, Range(-30, 30)]   public int Move;
   [SerializeField, Range(-0.85f, 0.85f)] public float Move2;
   [SerializeField, Range(-1f, 1f)] public float EndMove;

   private Rigidbody _currentBall;
   private Ball _ballScript;
   private Transform _nextTarget;
   
   private void Update()
   {
      var moveAbs = Math.Abs(EndMove);
      
      
      for (var i = 0; i < ListObjects.Length; i++)
      {
         if (ListObjects[i] == null)
            ListObjects[i] = Instantiate(Prefab);
         //y = ax^2 + bx + c
         L = Move;
         var added = A + Move2 * 0.005f;
         var x = i*2;
         var y = added * x * x + B * x + C;
         var z = (L / -15) * y;
         y = y + i*0.35f;
         ListObjects[i].transform.position = new Vector3(x, y,z);
      }
      
      if (_currentBall && !_ballScript.Collided)
      {
         MoveToTarget();
      }
   }

   private void MoveToTarget()
   {
      if (!_nextTarget)
         return;
      if (Vector3.Distance(_currentBall.position, _nextTarget.position) < 1)
      {
         _nextTarget = GetNextTarget();
      }
      if (_nextTarget)
         _currentBall.velocity = (_nextTarget.position - _currentBall.position).normalized * Speed;
   }


   private int _currentId;
   public void SpawnBall()
   {
      var ball = Instantiate(BallPrefab);
      _currentBall = ball.GetComponent<Rigidbody>();
      _ballScript = ball.GetComponent<Ball>();
      _currentId = 0;
      ball.transform.position = GetNextTarget().position;
      _nextTarget = GetNextTarget();
   }

   private Transform GetNextTarget()
   {
      if (_currentId < ListObjects.Length && ListObjects[_currentId])
      {
         var obj = ListObjects[_currentId].transform;
         _currentId++;
         return obj;
      }
      return null;
   }
}
