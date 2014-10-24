using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SG50
{
     class MyViewHolder : Java.Lang.Object
    {
        public TextView Title { get; set; }
        public TextView User { get; set; }
        public TextView Likes { get; set; }
        public VideoView Video { get; set; }
    }
}
