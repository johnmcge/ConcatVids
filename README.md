# ConcatVids
ConcatVids is another ffmpeg wrapper making it easy to concatenate a number of video clips into a single video file. It uses the -concat switch in ffmpeg, so each of the source video clips must be of the same form, ie same resolution, video/audio codec, frame rate, etc. A benefit of this approach is that the video is not re-encoded, ensuring no loss of quality and fast processing.

ConcatVids was created to help while working with output from home video cameras for sports events, concerts, etc.
 
In the sports example, a pattern that works well is to assemble all of the individual clips for a particular game into a directory, where the directory name has meaning (e.g. c:\video\incoming\DCIM\2018-01-01-Team1VsTeam2). Use other programs from this github account such as trim.exe to remove unwanted minutes/seconds from the begining and end of individual clips. Video cameras name files sequentially (e.g. MVI_0001.MP4, MVI_0002.MP4) so ConcatVids sorts the list of clips alphanumerically by filename. Running ConcatVids.exe with no optional parameters in a directory that has been prepared as described here will create a single video file by concatenating each of the clips in filename order, and the resulting video file will take the name of the current directory (e.g 2018-01-01-Team1vsTeam2.mp4). 
 
 Optional parameters:\
  &nbsp;&nbsp;&nbsp; -vpath PATH\
  &nbsp;&nbsp;&nbsp; set path to the source video files; will be current directory when not specified\

