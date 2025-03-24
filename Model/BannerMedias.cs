using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFFSSK.Model
{
    public class BannerMedias
    {
        public BannerMedias()
        {

        }

        public BannerMedias(int id, string name, string url, string desc, string remark, string fileType, string fileName, string extension)
        {
            this.BannerMediaId = id;
            this.BannerMediaName = name;
            this.FileUrl = url;
            this.Description = desc;
            this.Remark = remark;
            this.FileType = fileType;
            this.FileName = fileName;
            this.Extension = extension;
            this.FullFileName = fileName.Trim() + extension;
        }

        public int BannerMediaId { get; set; }
        public string BannerMediaName { get; set; }
        public string FileUrl { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string FullFileName { get; set; }

    }
}
