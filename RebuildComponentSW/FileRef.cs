using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

namespace RebuildComponentSW
{

    public class FileRef
    {
        private short mLocalOverWrittenVersionObsolete;
        private string mFileName;
        private string mPathName;
        private string mLatestVersion;
        private string mComponentLevel;
        private string mNumber;
        private string mDescription;
        private string mCurrentState;
      //  private string mCheckedOutBy;
        private List<FileRef> mFileRefs;

        public bool Equals(FileRef p)
        {
            if ((object)p == null)
            {
                return false;
            }

            return (mFileName == p.FileName);
        }

        public FileRef(string fileName, string pathName)
        {
            mFileName = fileName;
            mPathName = pathName;
            mFileRefs = new List<FileRef>();
        }

        public string PathName
        {
            get { return mPathName; }
            set { mPathName = value; }
        }

        public string FileName
        {
            get { return mFileName; }
            set { mFileName = value; }
        }

        public string LatestVersion
        {
            get { return mLatestVersion; }
            set { mLatestVersion = value; }
        }
        
        public string ComponentLevel
        {
            get { return mComponentLevel; }
            set { mComponentLevel = value; }
        }
        
        public string Number
        {
            get { return mNumber; }
            set { mNumber = value; }
        }

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }

        public string CurrentState
        {
            get { return mCurrentState; }
            set { mCurrentState = value; }
        }
        /*
        public string CheckedOutBy
        {
            get { return mCheckedOutBy; }
            set { mCheckedOutBy = value; }
        }
        */

        public short LocalOverWrittenVersionObsolete
        {
            get { return mLocalOverWrittenVersionObsolete; }
            set { mLocalOverWrittenVersionObsolete = value; }
        }

        public List<FileRef> FileRefs
        {
            get { return mFileRefs; }
            set { mFileRefs = value; }
        }
    }

}
