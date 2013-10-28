﻿//----------------------------------------------------------------------------
//  Copyright (C) 2004-2013 by EMGU. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Diagnostics;
using Emgu.CV.Cuda;

namespace PedestrianDetection
{
   public static class FindPedestrian
   {
      /// <summary>
      /// Find the pedestrian in the image
      /// </summary>
      /// <param name="image">The image</param>
      /// <param name="processingTime">The pedestrian detection time in milliseconds</param>
      /// <returns>The region where pedestrians are detected</returns>
      public static Rectangle[] Find(Image<Bgr, Byte> image, out long processingTime)
      {
         Stopwatch watch;
         Rectangle[] regions;

         //check if there is a compatible Cuda device to run pedestrian detection
         if (CudaInvoke.HasCuda)
         {  //this is the Cuda version
            using (CudaHOGDescriptor des = new CudaHOGDescriptor())
            {
               des.SetSVMDetector(CudaHOGDescriptor.GetDefaultPeopleDetector());

               watch = Stopwatch.StartNew();
               using (CudaImage<Bgr, Byte> cudaImg = new CudaImage<Bgr, byte>(image))
               using (CudaImage<Bgra, Byte> cudaBgra = cudaImg.Convert<Bgra, Byte>())
               {
                  regions = des.DetectMultiScale(cudaBgra);
               }
            }
         }
         else
         {  //this is the CPU version
            using (HOGDescriptor des = new HOGDescriptor())
            {
               des.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());

               watch = Stopwatch.StartNew();
               regions = des.DetectMultiScale(image);
            }
         }
         watch.Stop();

         processingTime = watch.ElapsedMilliseconds;

         return regions;
      }
   }
}
