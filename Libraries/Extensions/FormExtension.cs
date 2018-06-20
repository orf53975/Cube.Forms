﻿/* ------------------------------------------------------------------------- */
//
// Copyright (c) 2010 CubeSoft, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/* ------------------------------------------------------------------------- */
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Cube.Forms.Controls
{
    /* --------------------------------------------------------------------- */
    ///
    /// FormExtension
    ///
    /// <summary>
    /// System.Windows.Forms.Form の拡張用クラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public static class FormExtension
    {
        #region Methods

        #region UpdateCulture

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateCulture
        ///
        /// <summary>
        /// 表示言語を更新します。
        /// </summary>
        ///
        /// <param name="src">フォーム</param>
        /// <param name="name">表示言語名</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void UpdateCulture<T>(T src, string name) where T : Form
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(name);
            var rm = new ComponentResourceManager(typeof(T));
            rm.ApplyResources(src, "$this");
            src.Controls.UpdateCulture(rm);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateCulture
        ///
        /// <summary>
        /// 表示言語を更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static void UpdateCulture(this Control.ControlCollection src, ComponentResourceManager rm)
        {
            foreach (Control control in src)
            {
                rm.ApplyResources(control, control.Name);
                control.Controls.UpdateCulture(rm);
            }
        }

        #endregion

        #region UpdateText

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateText
        ///
        /// <summary>
        /// フォームのタイトルを "message - ProductName" と言う表記で
        /// 更新します。
        /// </summary>
        ///
        /// <param name="src">フォーム</param>
        /// <param name="message">タイトルに表示するメッセージ</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void UpdateText(this Form src, string message) =>
            UpdateText(src, message, AssemblyReader.Default);

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateText
        ///
        /// <summary>
        /// フォームのタイトルを "message - ProductName" と言う表記で
        /// 更新します。
        /// </summary>
        ///
        /// <param name="src">フォーム</param>
        /// <param name="message">タイトルに表示するメッセージ</param>
        /// <param name="assembly">アセンブリ情報</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void UpdateText(this Form src, string message, Assembly assembly) =>
            UpdateText(src, message, new AssemblyReader(assembly));

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateText
        ///
        /// <summary>
        /// フォームのタイトルを "message - ProductName" と言う表記で
        /// 更新します。
        /// </summary>
        ///
        /// <param name="src">フォーム</param>
        /// <param name="message">タイトルに表示するメッセージ</param>
        /// <param name="assembly">アセンブリ情報</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void UpdateText(this Form src, string message, AssemblyReader assembly)
        {
            var ss = new System.Text.StringBuilder();
            ss.Append(message);
            if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(assembly.Product)) ss.Append(" - ");
            ss.Append(assembly.Product);

            src.Text = ss.ToString();
        }

        #endregion

        #region TopMost

        /* ----------------------------------------------------------------- */
        ///
        /// BringToFront
        ///
        /// <summary>
        /// フォームを最前面に表示します。
        /// </summary>
        ///
        /// <param name="src">フォーム</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void BringToFront(this Form src)
        {
            src.ResetTopMost();
            src.Activate();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ResetTopMost
        ///
        /// <summary>
        /// TopMost の値をリセットします。
        /// </summary>
        ///
        /// <param name="src">フォーム</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void ResetTopMost(this Form src)
        {
            var tmp = src.TopMost;
            src.TopMost = false;
            src.TopMost = true;
            src.TopMost = tmp;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SetTopMost
        ///
        /// <summary>
        /// フォームを最前面に表示します。
        /// </summary>
        ///
        /// <param name="src">フォーム</param>
        /// <param name="active">アクティブ状態にするかどうか</param>
        ///
        /// <remarks>
        /// SetTopMost メソッドは主にフォーカスを奪わずに最前面に表示する
        /// 時に使用します。この場合、最前面に表示された状態でも TopMost
        /// プロパティは false となります。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public static void SetTopMost(this Form src, bool active)
        {
            if (active)
            {
                src.TopMost = true;
                src.Activate();
            }
            else src.SetTopMostWithoutActivate();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SetTopMostWithoutActivate
        ///
        /// <summary>
        /// フォームを非アクティブ状態で最前面に表示します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static void SetTopMostWithoutActivate(this Form src) =>
            User32.NativeMethods.SetWindowPos(src.Handle,
                (IntPtr)(-1), /* HWND_TOPMOST */
                0, 0, 0, 0,
                0x0413 // SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSENDCHANGING | SWP_NOSIZE
            );

        #endregion

        #endregion
    }
}