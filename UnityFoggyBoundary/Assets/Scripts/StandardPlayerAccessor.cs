using System;
using System.Reflection;
using UnityEngine;
using Controller = UnityStandardAssets.Characters.FirstPerson.FirstPersonController;

namespace VDV.FoggyBoundary
{
    [RequireComponent(typeof(Controller))]
    public class StandardPlayerAccessor : PlayerAccessor
    {
        private Controller controller;
        private FieldInfo moveDirField;
        private FieldInfo runSpeedField;
        private FieldInfo walkSpeedField;

        public override float WalkSpeed
        {
            get { return (float) walkSpeedField.GetValue(controller); }
            set { walkSpeedField.SetValue(controller, value); }
        }
        public override float RunSpeed
        {
            get { return (float)runSpeedField.GetValue(controller); }
            set { runSpeedField.SetValue(controller, value); }
        }
        public override Vector3 MoveDir
        {
            get { return (Vector3)moveDirField.GetValue(controller); }
            set { moveDirField.SetValue(controller, value); }
        }

        private void Start()
        {
            controller = GetComponent<Controller>();
            Type type = typeof(Controller);
            walkSpeedField = type.GetField("m_WalkSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            runSpeedField = type.GetField("m_RunSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            moveDirField = type.GetField("m_MoveDir", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}