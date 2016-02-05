﻿/* ------------------------------------------------------------------------- */
///
/// ListView.cs
/// 
/// Copyright (c) 2010 CubeSoft, Inc.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///  http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
///
/* ------------------------------------------------------------------------- */
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace Cube.Forms
{
    /* --------------------------------------------------------------------- */
    ///
    /// ListView
    /// 
    /// <summary>
    /// リストビューを表示するクラスです。
    /// </summary>
    /// 
    /* --------------------------------------------------------------------- */
    public class ListView : System.Windows.Forms.ListView
    {
        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Theme
        ///
        /// <summary>
        /// 表示用のテーマを取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Browsable(true)]
        [DefaultValue(WindowTheme.Normal)]
        public WindowTheme Theme
        {
            get { return _theme; }
            set
            {
                if (_theme != value)
                {
                    _theme = value;
                    UpdateTheme(_theme);
                }
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Count
        ///
        /// <summary>
        /// 項目数を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int Count
        {
            get
            {
                return VirtualMode ? VirtualListSize : Items.Count;

            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// AnyItemsSelected
        ///
        /// <summary>
        /// 項目を 1 つ以上選択しているかどうかを示す値を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool AnyItemsSelected
        {
            get { return SelectedIndices != null && SelectedIndices.Count > 0; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Converter
        ///
        /// <summary>
        /// 変換用オブジェクトを取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IListViewItemConverter Converter { get; set; }

        #endregion

        #region Events

        /* ----------------------------------------------------------------- */
        ///
        /// Added
        ///
        /// <summary>
        /// 項目を追加する直前に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event EventHandler<DataCancelEventArgs<int>> Adding;

        /* ----------------------------------------------------------------- */
        ///
        /// Added
        ///
        /// <summary>
        /// 項目が追加された時に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event EventHandler<DataEventArgs<int>> Added;

        /* ----------------------------------------------------------------- */
        ///
        /// Repllacing
        ///
        /// <summary>
        /// 項目が置換される直前に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event EventHandler<DataCancelEventArgs<int>> Replacing;

        /* ----------------------------------------------------------------- */
        ///
        /// Replaced
        ///
        /// <summary>
        /// 項目が置換された時に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event EventHandler<DataEventArgs<int>> Replaced;

        /* ----------------------------------------------------------------- */
        ///
        /// Removing
        ///
        /// <summary>
        /// 項目が削除される直前に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event EventHandler<DataCancelEventArgs<int[]>> Removing;

        /* ----------------------------------------------------------------- */
        ///
        /// Removed
        ///
        /// <summary>
        /// 項目が削除された時に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event EventHandler<DataEventArgs<int[]>> Removed;

        /* ----------------------------------------------------------------- */
        ///
        /// Clearing
        ///
        /// <summary>
        /// 全ての項目が削除される直前に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event EventHandler<CancelEventArgs> Clearing;

        /* ----------------------------------------------------------------- */
        ///
        /// Cleared
        ///
        /// <summary>
        /// 全ての項目が削除された時に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event EventHandler Cleared;

        /* ----------------------------------------------------------------- */
        ///
        /// Moving
        ///
        /// <summary>
        /// 項目が移動される直前に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event EventHandler<CancelEventArgs> Moving;

        /* ----------------------------------------------------------------- */
        ///
        /// Moved
        ///
        /// <summary>
        /// 項目が移動された時に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event EventHandler<MoveEventArgs> Moved;

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Select
        ///
        /// <summary>
        /// 項目を選択します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Select(int index)
        {
            if (index < 0 || index >= Count) return;
            SelectedIndices.Clear();
            SelectedIndices.Add(index);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Select
        ///
        /// <summary>
        /// 項目を選択します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Select(IEnumerable<int> indices)
        {
            SelectedIndices.Clear();
            foreach (var index in indices)
            {
                if (index < 0 || index >= Count) continue;
                SelectedIndices.Add(index);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SelectMore
        ///
        /// <summary>
        /// 既に選択されている項目を保持したまま、項目を選択します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void SelectMore(int index)
        {
            if (index < 0 || index >= Count) return;
            if (SelectedIndices.Contains(index)) return;
            SelectedIndices.Add(index);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SelectMore
        ///
        /// <summary>
        /// 既に選択されている項目を保持したまま、項目を選択します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void SelectMore(IEnumerable<int> indices)
        {
            foreach (var index in indices)
            {
                if (index < 0 || index >= Count) return;
                if (SelectedIndices.Contains(index)) return;
                SelectedIndices.Add(index);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Add
        ///
        /// <summary>
        /// 項目を追加します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Add<T>(T item)
        {
            var index = Count;

            var args = new DataCancelEventArgs<int>(index);
            OnAdding(args);
            if (args.Cancel || VirtualMode) return;

            Items.Add(
                Converter != null ?
                Converter.Convert(item) :
                new System.Windows.Forms.ListViewItem(item.ToString())
            );
            HackAlignmentBug();

            OnAdded(new DataEventArgs<int>(index));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Insert
        ///
        /// <summary>
        /// 指定されたインデックスに項目を追加します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Insert<T>(int index, T item)
        {
            var args = new DataCancelEventArgs<int>(index);
            OnAdding(args);
            if (args.Cancel || VirtualMode) return;

            Items.Insert(index,
                Converter != null ?
                Converter.Convert(item) :
                new System.Windows.Forms.ListViewItem(item.ToString())
            );
            HackAlignmentBug();

            OnAdded(new DataEventArgs<int>(index));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Replace
        ///
        /// <summary>
        /// 指定されたインデックスの内容を置換します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Replace<T>(int index, T item)
        {
            if (index < 0 || index >= Count) return;
            var args = new DataCancelEventArgs<int>(index);
            OnReplacing(args);
            if (args.Cancel || VirtualMode) return;

            Items[index] = Converter != null ?
                           Converter.Convert(item) :
                           new System.Windows.Forms.ListViewItem(item.ToString());

            OnReplaced(new DataEventArgs<int>(index));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RemoveItems
        ///
        /// <summary>
        /// 項目を削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void RemoveItems(IEnumerable<int> indices)
        {
            var args = new DataCancelEventArgs<int[]>(indices.ToArray());
            OnRemoving(args);
            if (args.Cancel || VirtualMode) return;

            foreach (var index in indices.OrderByDescending(x => x))
            {
                if (index < 0 || index >= Count) continue;
                Items.RemoveAt(index);
            }

            OnRemoved(new DataEventArgs<int[]>(indices.ToArray()));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RemoveItems
        ///
        /// <summary>
        /// 選択されている項目を削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void RemoveItems()
        {
            var indices = new List<int>();
            foreach (int index in SelectedIndices) indices.Add(index);
            RemoveItems(indices);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ClearItems
        ///
        /// <summary>
        /// 全ての項目を削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void ClearItems()
        {
            var args = new CancelEventArgs();
            OnClearing(args);
            if (args.Cancel || VirtualMode) return;

            Items.Clear();
            OnCleared(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// MoveItems
        ///
        /// <summary>
        /// 項目を移動します。
        /// </summary>
        /// 
        /// <remarks>
        /// offset が正の数の場合は後ろに、負の数の場合は前に移動します。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public void MoveItems(IEnumerable<int> indices, int offset)
        {
            if (offset == 0) return;

            var args = new MoveCancelEventArgs(indices.ToArray(), offset);
            OnMoving(args);
            if (args.Cancel || VirtualMode) return;

            var sorted = offset > 0 ?
                         indices.OrderByDescending(x => x) :
                         indices.OrderBy(x => x);

            foreach (var index in sorted)
            {
                if (index < 0 || index >= Count) continue;
                var item = Items[index];
                Items.RemoveAt(index);

                var newindex = Math.Max(Math.Min(index + offset, Count), 0);
                Items.Insert(newindex, item);
            }

            OnMoved(new MoveEventArgs(indices.ToArray(), offset));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// MoveItems
        ///
        /// <summary>
        /// 選択されている項目を移動します。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        public void MoveItems(int offset)
        {
            var indices = new List<int>();
            foreach (int index in SelectedIndices) indices.Add(index);
            MoveItems(indices, offset);
        }

        #endregion

        #region Virtual methods

        /* ----------------------------------------------------------------- */
        ///
        /// OnAdding
        ///
        /// <summary>
        /// Adding イベントを発生させます。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        protected virtual void OnAdding(DataCancelEventArgs<int> e)
        {
            if (Adding != null) Adding(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnAdded
        ///
        /// <summary>
        /// Added イベントを発生させます。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        protected virtual void OnAdded(DataEventArgs<int> e)
        {
            if (Added != null) Added(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnReplacing
        ///
        /// <summary>
        /// Replacing イベントを発生させます。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        protected virtual void OnReplacing(DataCancelEventArgs<int> e)
        {
            if (Replacing != null) Replacing(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnReplaced
        ///
        /// <summary>
        /// Replaced イベントを発生させます。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        protected virtual void OnReplaced(DataEventArgs<int> e)
        {
            if (Replaced != null) Replaced(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnRemoving
        ///
        /// <summary>
        /// Removing イベントを発生させます。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        protected virtual void OnRemoving(DataCancelEventArgs<int[]> e)
        {
            if (Removing != null) Removing(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnRemoved
        ///
        /// <summary>
        /// Removed イベントを発生させます。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        protected virtual void OnRemoved(DataEventArgs<int[]> e)
        {
            if (Removed != null) Removed(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnClearing
        ///
        /// <summary>
        /// Clearing イベントを発生させます。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        protected virtual void OnClearing(CancelEventArgs e)
        {
            if (Clearing != null) Clearing(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnCleared
        ///
        /// <summary>
        /// Cleared イベントを発生させます。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        protected virtual void OnCleared(EventArgs e)
        {
            if (Cleared != null) Cleared(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnMoving
        ///
        /// <summary>
        /// Moving イベントを発生させます。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        protected virtual void OnMoving(MoveCancelEventArgs e)
        {
            if (Moving != null) Moving(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnMoved
        ///
        /// <summary>
        /// Moved イベントを発生させます。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        protected virtual void OnMoved(MoveEventArgs e)
        {
            if (Moved != null) Moved(this, e);
        }

        #endregion

        #region Override methods

        /* ----------------------------------------------------------------- */
        ///
        /// OnCreateControl
        ///
        /// <summary>
        /// コントロールの生成時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected override void OnCreateControl()
        {
            UpdateTheme(_theme);
            base.OnCreateControl();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnSelectedIndexChanged
        ///
        /// <summary>
        /// 選択項目が変更された時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            if (Theme == WindowTheme.Normal) return;
            User32.SendMessage(Handle, 0x127, (IntPtr)0x10001, IntPtr.Zero);
        }
        /* ----------------------------------------------------------------- */
        ///
        /// OnEnter
        ///
        /// <summary>
        /// カーソルが領域内に侵入した時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (Theme == WindowTheme.Normal) return;
            User32.SendMessage(Handle, 0x127, (IntPtr)0x10001, IntPtr.Zero);
        }

        #endregion

        #region Others

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateTheme
        ///
        /// <summary>
        /// 表示用のテーマを更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UpdateTheme(WindowTheme theme)
        {
            if (theme == WindowTheme.Normal) UxTheme.SetWindowTheme(Handle, null, null);
            else UxTheme.SetWindowTheme(Handle, theme.ToString(), null);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// HackAlignmentBug
        ///
        /// <summary>
        /// Aligment に関連したバグの影響で表示順序がおかしくなる場合が
        /// あるので、強制的に再描画させます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void HackAlignmentBug()
        {
            if (View == System.Windows.Forms.View.List ||
                View == System.Windows.Forms.View.Details) return;

            var alignment = Alignment;
            Alignment = System.Windows.Forms.ListViewAlignment.Default;
            Alignment = alignment;
        }

        #endregion

        #region Classes

        /* ----------------------------------------------------------------- */
        ///
        /// MoveEventArgs
        ///
        /// <summary>
        /// Move イベントのデータを保持するクラスです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public class MoveEventArgs : EventArgs
        {
            public MoveEventArgs(int[] indices, int offset) : base()
            {
                Indices = indices;
                Offset  = offset;
            }

            public int[] Indices { get; }
            public int Offset { get; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// MoveCancelEventArgs
        ///
        /// <summary>
        /// Move イベントのデータを保持するクラスです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public class MoveCancelEventArgs : CancelEventArgs
        {
            public MoveCancelEventArgs(int[] indices, int offset) : base(false)
            {
                Indices = indices;
                Offset = offset;
            }

            public int[] Indices { get; }
            public int Offset { get; }
        }

        #endregion

        #region Fields
        private WindowTheme _theme = WindowTheme.Normal;
        #endregion
    }
}