﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Pos.Internals.Extensions
{
    #region Public Enumerations

    #endregion
    /// <summary>
    /// Allows for retrieval of large icons from files.
    /// </summary>
    public class SysImageList : IDisposable
    {
        #region Constants
        
        /// <summary>
        /// Maximum path.
        /// </summary>
        private const int MAXPATH = 260;
        private const int FILEATTRIBUTENORMAL = 0x80;
        private const int FILEATTRIBUTEDIRECTORY = 0x10;
        private const int FORMATMESSAGEALLOCATEBUFFER = 0x100;
        private const int FORMATMESSAGEARGUMENTARRAY = 0x2000;
        private const int FORMATMESSAGEFROMHMODULE = 0x800;
        private const int FORMATMESSAGEFROMSTRING = 0x400;
        private const int FORMATMESSAGEFROMSYSTEM = 0x1000;
        private const int FORMATMESSAGEIGNOREINSERTS = 0x200;
        private const int FORMATMESSAGEMAXWIDTHMASK = 0xFF;
        
        #endregion
        
        #region Member Variables
        
        private IntPtr himl = IntPtr.Zero;
        private IImageList imageList = null;
        private SysImageListSize size = SysImageListSize.smallIcons;
        private bool disposed = false;
        
        #endregion
        
        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the SysImageList class.
        /// </summary>
        public SysImageList()
        {
            this.Create();
        }
        
        /// <summary>
        /// Initializes a new instance of the SysImageList class.
        /// </summary>
        /// <param name="size">Size of System ImageList</param>
        public SysImageList(SysImageListSize size)
        {
            this.size = size;
            this.Create();
        }
        
        #endregion
        
        #region Destructor
        
        /// <summary>
        /// Finalizes an instance of the SysImageList class.
        /// </summary>
        ~SysImageList()
        {
            this.Dispose(false);
        }
        
        #endregion
        
        #region Private Enumerations
        
        [Flags]
        private enum SHGetFileInfoConstants : int
        {
            // get icon 
            // get display name 
            // get type name 
            // get attributes 
            // get icon location 
            // return exe type 
            SHGFI_SYSICONINDEX = 0x4000,       // get system icon index 
            // put a link overlay on icon 
            // show icon in selected state 
            // get only specified attributes 
            // get large icon 
            SHGFI_SMALLICON = 0x1,             // get small icon 
            // get open icon 
            // get shell size icon
            SHGFI_USEFILEATTRIBUTES = 0x10,    // use passed dwFileAttribute 
            // Get the index of the overlay
        }
        
        #endregion
        
        #region Private Interfaces
        
        [ComImport()]
        [Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IImageList
        {
            [PreserveSig]
            int Add(
                IntPtr maskedImage,
                IntPtr hbmMask,
                ref int pi);
            
            [PreserveSig]
            int ReplaceIcon(
                int i,
                IntPtr hicon,
                ref int pi);
            
            [PreserveSig]
            int SetOverlayImage(
                int image,
                int overlay);
            
            [PreserveSig]
            int Replace(
                int i,
                IntPtr maskedImage,
                IntPtr mask);
            
            [PreserveSig]
            int AddMasked(
                IntPtr maskedImage,
                int mask,
                ref int pi);
            
            [PreserveSig]
            int Draw(
                ref IMAGELISTDRAWPARAMS pimldp);
            
            [PreserveSig]
            int Remove(
                int i);
            
            [PreserveSig]
            int GetIcon(
                int i,
                int flags,
                ref IntPtr picon);
            
            [PreserveSig]
            int GetImageInfo(
                int i,
                ref IMAGEINFO imageInfo);
            
            [PreserveSig]
            int Copy(
                int destination,
                IImageList punkSrc,
                int source,
                int flags);
            
            [PreserveSig]
            int Merge(
                int i1,
                IImageList punk2,
                int i2,
                int dx,
                int dy,
                ref Guid riid,
                ref IntPtr ppv);
            
            [PreserveSig]
            int Clone(
                ref Guid riid,
                ref IntPtr ppv);
            
            [PreserveSig]
            int GetImageRect(
                int i,
                ref RECT prc);
            
            [PreserveSig]
            int GetIconSize(
                ref int cx,
                ref int cy);
            
            [PreserveSig]
            int SetIconSize(
                int cx,
                int cy);
            
            [PreserveSig]
            int GetImageCount(
                ref int pi);
            
            [PreserveSig]
            int SetImageCount(
                int newCount);
            
            [PreserveSig]
            int SetBkColor(
                int clrBk,
                ref int pclr);
            
            [PreserveSig]
            int GetBkColor(
                ref int pclr);
            
            [PreserveSig]
            int BeginDrag(
                int track,
                int hotspotX,
                int hotspotY);

            [PreserveSig]
            int EndDrag();
            
            [PreserveSig]
            int DragEnter(
                IntPtr lockHandle,
                int x,
                int y);
            
            [PreserveSig]
            int DragLeave(
                IntPtr lockHandle);
            
            [PreserveSig]
            int DragMove(
                int x,
                int y);
            
            [PreserveSig]
            int SetDragCursorImage(
                ref IImageList punk,
                int drag,
                int hotspotX,
                int hotspotY);
            
            [PreserveSig]
            int DragShowNolock(
                int show);
            
            [PreserveSig]
            int GetDragImage(
                ref POINT ppt,
                ref POINT pptHotspot,
                ref Guid riid,
                ref IntPtr ppv);
            
            [PreserveSig]
            int GetItemFlags(
                int i,
                ref int flags);
            
            [PreserveSig]
            int GetOverlayImage(
                int overlay,
                ref int index);
        }
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Gets the hImageList handle
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                return this.himl;
            }
        }
        
        /// <summary>
        /// Gets or sets the size of System Image List to retrieve.
        /// </summary>
        public SysImageListSize ImageListSize
        {
            get
            {
                return this.size;
            }
            
            set
            {
                this.size = value;
                
                this.Create();
            }
        }
        
        /// <summary>
        /// Gets the size of the Image List Icons.
        /// </summary>
        public System.Drawing.Size Size
        {
            get
            {
                int cx = 0;
                int cy = 0;
                
                if (this.imageList == null)
                {
                    ImageList_GetIconSize(this.himl, ref cx, ref cy);
                }
                else
                {
                    this.imageList.GetIconSize(ref cx, ref cy);
                }
                
                System.Drawing.Size sz = new System.Drawing.Size(cx, cy);
                
                return sz;
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Clears up any resources associated with the SystemImageList
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Clears up any resources associated with the SystemImageList
        /// when disposing is true.
        /// </summary>
        /// <param name="disposing">Whether the object is being disposed</param>
        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.imageList != null)
                    {
                        Marshal.ReleaseComObject(this.imageList);
                    }
                    
                    this.imageList = null;
                }
            }

            this.disposed = true;
        }
        
        /// <summary>
        /// Returns a GDI+ copy of the icon from the ImageList
        /// at the specified index.
        /// </summary>
        /// <param name="index">The index to get the icon for</param>
        /// <returns>The specified icon</returns>
        public Icon Icon(int index)
        {
            Icon icon = null;
            
            IntPtr handleIcon = IntPtr.Zero;
            if (this.imageList == null)
            {
                handleIcon = ImageList_GetIcon(this.himl, index, (int)ImageListDrawItemConstants.ILD_TRANSPARENT);
            }
            else
            {
                this.imageList.GetIcon(index, (int)ImageListDrawItemConstants.ILD_TRANSPARENT, ref handleIcon);
            }
            
            if (handleIcon != IntPtr.Zero)
            {
                icon = System.Drawing.Icon.FromHandle(handleIcon);
            }

            return icon;
        }
        
        /// <summary>
        /// Return the index of the icon for the specified file, always using 
        /// the cached version where possible.
        /// </summary>
        /// <param name="fileName">Filename to get icon for</param>
        /// <returns>Index of the icon</returns>
        public int IconIndex(string fileName)
        {
            return this.IconIndex(fileName, false);
        }
        
        /// <summary>
        /// Returns the index of the icon for the specified file
        /// </summary>
        /// <param name="fileName">Filename to get icon for</param>
        /// <param name="forceLoadFromDisk">If True, then hit the disk to get the icon,
        /// otherwise only hit the disk if no cached icon is available.</param>
        /// <returns>Index of the icon</returns>
        public int IconIndex(string fileName, bool forceLoadFromDisk)
        {
            return this.IconIndex(fileName, forceLoadFromDisk, ShellIconStateConstants.ShellIconStateNormal);
        }
        
        /// <summary>
        /// Returns the index of the icon for the specified file
        /// </summary>
        /// <param name="fileName">Filename to get icon for</param>
        /// <param name="forceLoadFromDisk">If True, then hit the disk to get the icon,
        /// otherwise only hit the disk if no cached icon is available.</param>
        /// <param name="iconState">Flags specifying the state of the icon
        /// returned.</param>
        /// <returns>Index of the icon</returns>
        public int IconIndex(string fileName, bool forceLoadFromDisk, ShellIconStateConstants iconState)
        {
            SHGetFileInfoConstants flags = SHGetFileInfoConstants.SHGFI_SYSICONINDEX;
            
            int attr = 0;
            
            if (this.size == SysImageListSize.smallIcons)
            {
                flags |= SHGetFileInfoConstants.SHGFI_SMALLICON;
            }
            
            // We can choose whether to access the disk or not. If you don't
            // hit the disk, you may get the wrong icon if the icon is
            // not cached. Also only works for files.
            if (!forceLoadFromDisk)
            {
                flags |= SHGetFileInfoConstants.SHGFI_USEFILEATTRIBUTES;
                attr = FILEATTRIBUTENORMAL;
            }
            else
            {
                attr = 0;
            }
            
            // sFileSpec can be any file. You can specify a
            // file that does not exist and still get the
            // icon, for example sFileSpec = "C:\PANTS.DOC"
            SHFILEINFO shfi = new SHFILEINFO();
            
            uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
            
            IntPtr retVal = SHGetFileInfo(fileName, attr, ref shfi, shfiSize, (uint)flags | (uint)iconState);
            
            if (retVal.Equals(IntPtr.Zero))
            {
                System.Diagnostics.Debug.Assert((!retVal.Equals(IntPtr.Zero)), "Failed to get icon index");
                
                return 0;
            }
            else
            {
                return shfi.Icon;
            }
        }
        
        /// <summary>
        /// Draws an image
        /// </summary>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        public void DrawImage(IntPtr hdc, int index, int x, int y)
        {
            this.DrawImage(hdc, index, x, y, ImageListDrawItemConstants.ILD_TRANSPARENT);
        }
        
        /// <summary>
        /// Draws an image using the specified flags
        /// </summary>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        /// <param name="flags">Drawing flags</param>
        public void DrawImage(IntPtr hdc, int index, int x, int y, ImageListDrawItemConstants flags)
        {
            if (this.imageList == null)
            {
                int ret = ImageList_Draw(
                    this.himl,
                    index,
                    hdc,
                    x,
                    y,
                    (int)flags);
            }
            else
            {
                IMAGELISTDRAWPARAMS pimldp = new IMAGELISTDRAWPARAMS();
                pimldp.HdcDst = hdc;
                pimldp.Size = Marshal.SizeOf(pimldp.GetType());
                pimldp.Index = index;
                pimldp.X = x;
                pimldp.Y = y;
                pimldp.ForegroundRgb = -1;
                pimldp.Style = (int)flags;
                this.imageList.Draw(ref pimldp);
            }
        }
        
        /// <summary>
        /// Draws an image using the specified flags and specifies
        /// the size to clip to (or to stretch to if ILD_SCALE
        /// is provided).
        /// </summary>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        /// <param name="flags">Drawing flags</param>
        /// <param name="cx">Width to draw</param>
        /// <param name="cy">Height to draw</param>
        public void DrawImage(IntPtr hdc, int index, int x, int y, ImageListDrawItemConstants flags, int cx, int cy)
        {
            IMAGELISTDRAWPARAMS pimldp = new IMAGELISTDRAWPARAMS();
            pimldp.HdcDst = hdc;
            pimldp.Size = Marshal.SizeOf(pimldp.GetType());
            pimldp.Index = index;
            pimldp.X = x;
            pimldp.Y = y;
            pimldp.CX = cx;
            pimldp.CY = cy;
            pimldp.Style = (int)flags;
            
            if (this.imageList == null)
            {
                pimldp.Himl = this.himl;
                
                int ret = ImageList_DrawIndirect(ref pimldp);
            }
            else
            {
                this.imageList.Draw(ref pimldp);
            }
        }
        
        /// <summary>
        /// Draws an image using the specified flags and state on XP systems.
        /// </summary>
        /// <param name="hdc">Device context to draw to</param>
        /// <param name="index">Index of image to draw</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        /// <param name="flags">Drawing flags</param>
        /// <param name="cx">Width to draw</param>
        /// <param name="cy">Height to draw</param>
        /// <param name="foreColor">Fore colour to blend with when using the ILD_SELECTED or ILD_BLEND25 flags</param>
        /// <param name="stateFlags">State flags</param>
        /// <param name="saturateColorOrAlpha">If stateFlags includes ILS_ALPHA, then the alpha component is applied to the icon. Otherwise if 
        /// ILS_SATURATE is included, then the (R,G,B) components are used to saturate the image.</param>
        /// <param name="glowOrShadowColor">If stateFlags include ILS_GLOW, then the colour to use for the glow effect.  Otherwise if stateFlags includes 
        /// ILS_SHADOW, then the colour to use for the shadow.</param>
        public void DrawImage(
            IntPtr hdc,
            int index,
            int x,
            int y,
            ImageListDrawItemConstants flags,
            int cx,
            int cy,
            System.Drawing.Color foreColor,
            ImageListDrawStateConstants stateFlags,
            System.Drawing.Color saturateColorOrAlpha,
            System.Drawing.Color glowOrShadowColor)
        {
            IMAGELISTDRAWPARAMS pimldp = new IMAGELISTDRAWPARAMS();
            pimldp.HdcDst = hdc;
            pimldp.Size = Marshal.SizeOf(pimldp.GetType());
            pimldp.Index = index;
            pimldp.X = x;
            pimldp.Y = y;
            pimldp.CX = cx;
            pimldp.CY = cy;
            pimldp.ForegroundRgb = Color.FromArgb(0, foreColor.R, foreColor.G, foreColor.B).ToArgb();
            pimldp.Style = (int)flags;
            pimldp.State = (int)stateFlags;
            if ((stateFlags & ImageListDrawStateConstants.ILS_ALPHA) == ImageListDrawStateConstants.ILS_ALPHA)
            {
                // Set the alpha
                pimldp.Frame = saturateColorOrAlpha.A;
            }
            else if ((stateFlags & ImageListDrawStateConstants.ILS_SATURATE) == ImageListDrawStateConstants.ILS_SATURATE)
            {
                // discard alpha channel:
                saturateColorOrAlpha = Color.FromArgb(0, saturateColorOrAlpha.R, saturateColorOrAlpha.G, saturateColorOrAlpha.B);
                
                // set the saturate color
                pimldp.Frame = saturateColorOrAlpha.ToArgb();
            }
            
            glowOrShadowColor = Color.FromArgb(0, glowOrShadowColor.R, glowOrShadowColor.G, glowOrShadowColor.B);
            
            pimldp.Effect = glowOrShadowColor.ToArgb();
            
            if (this.imageList == null)
            {
                pimldp.Himl = this.himl;
                
                int ret = ImageList_DrawIndirect(ref pimldp);
            }
            else
            {
                this.imageList.Draw(ref pimldp);
            }
        }
        
        #endregion
        
        #region Private Methods

        [DllImport("user32.dll")]
        private static extern int DestroyIcon(IntPtr handleIcon);
        
        [DllImport("shell32")]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            int fileAttributes,
            ref SHFILEINFO psfi,
            uint fileInfo,
            uint flags);
        
        [DllImport("kernel32")]
        private static extern int FormatMessage(
            int flags,
            IntPtr source,
            int messageId,
            int languageId,
            string buffer,
            uint size,
            int argumentsLong);

        [DllImport("kernel32")]
        private static extern int GetLastError();
        
        [DllImport("comctl32")]
        private static extern int ImageList_Draw(
            IntPtr himl,
            int i,
            IntPtr hdcDst,
            int x,
            int y,
            int style);
        
        [DllImport("comctl32")]
        private static extern int ImageList_DrawIndirect(
            ref IMAGELISTDRAWPARAMS pimldp);
        
        [DllImport("comctl32")]
        private static extern int ImageList_GetIconSize(
            IntPtr himl,
            ref int cx,
            ref int cy);
        
        [DllImport("comctl32")]
        private static extern IntPtr ImageList_GetIcon(
            IntPtr himl,
            int i,
            int flags);
        
        /// <summary>
        /// SHGetImageList is not exported correctly in XP.  See KB316931
        /// http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316931
        /// Apparently (and hopefully) ordinal 727 isn't going to change.
        /// </summary>
        /// <param name="imageList">Image list.</param>
        /// <param name="riid">Globally unique ID.</param>
        /// <param name="ppv">Image list.</param>
        /// <returns>Resulting integer.</returns>
        [DllImport("shell32.dll", EntryPoint = "#727")]
        private static extern int SHGetImageList(
            int imageList,
            ref Guid riid,
            ref IImageList ppv);
        
        [DllImport("shell32.dll", EntryPoint = "#727")]
        private static extern int SHGetImageListHandle(
            int imageList,
            ref Guid riid,
            ref IntPtr handle);
        
        /// <summary>
        /// Creates the SystemImageList
        /// </summary>
        private void Create()
        {
            // forget last image list if any:
            this.himl = IntPtr.Zero;
            
            if (SystemUtilities.IsXPOrAbove())
            {
                // Get the System IImageList object from the Shell:
                Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
                int ret = SHGetImageList(
                    (int)this.size,
                    ref iidImageList,
                    ref this.imageList);
                
                // the image list handle is the IUnknown pointer, but 
                // using Marshal.GetIUnknownForObject doesn't return
                // the right value.  It really doesn't hurt to make
                // a second call to get the handle:
                SHGetImageListHandle((int)this.size, ref iidImageList, ref this.himl);
            }
            else
            {
                // Prepare flags:
                SHGetFileInfoConstants flags = SHGetFileInfoConstants.SHGFI_USEFILEATTRIBUTES | SHGetFileInfoConstants.SHGFI_SYSICONINDEX;
                if (this.size == SysImageListSize.smallIcons)
                {
                    flags |= SHGetFileInfoConstants.SHGFI_SMALLICON;
                }
                
                // Get image list
                SHFILEINFO shfi = new SHFILEINFO();
                uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
                
                // Call SHGetFileInfo to get the image list handle
                // using an arbitrary file:
                this.himl = SHGetFileInfo(".txt", FILEATTRIBUTENORMAL, ref shfi, shfiSize, (uint)flags);
                
                System.Diagnostics.Debug.Assert((this.himl != IntPtr.Zero), "Failed to create Image List");
            }
        }
        
        #endregion
        
        #region Private Structures
        
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGELISTDRAWPARAMS
        {
            public int Size;
            public IntPtr Himl;
            public int Index;
            public IntPtr HdcDst;
            public int X;
            public int Y;
            public int CX;
            public int CY;
            // x offest from the upperleft of bitmap
            // y offset from the upperleft of bitmap
            public int ForegroundRgb;
            public int Style;
            public int State;
            public int Frame;
            public int Effect;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGEINFO
        {
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public readonly int Icon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPATH)]
            public string DisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string TypeName;
        }
        #endregion
    }
}