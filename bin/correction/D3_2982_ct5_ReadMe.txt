SCANLAB RTC 3D Correction File
==============================

This ct5 correction file is calculated for a SCANLAB 3-axis scan system
for 3D image field correction. The ct5 format succeeds the ctb correction
file format, which is used with the RTC2 to RTC4. For further information
please refer to the RTC manual.


3D Correction File Parameters
-----------------------------

Filename:                     D3_2982.ct5
Program Version:              4.0.3
Date:                         08.09.2022
Description:                  3D Correction File With F-Theta-Lens 
varioSCAN Article Number:     152612
Scanning Lens:                120720
-
Evaluation Wavelength:        355 nm


Scan Head Type:               s14        
Scan Angle Calibration:       +/- 11.7 degrees mech.
XY-Swap:                      No

Scan Field Calibration K_xy:  8656 bit/mm
Scan Field Calibration K_z:   16-bit: 541 bit/mm | 20-bit: 8656 bit/mm
Max. Field Size (z=0):        121.139 mm
Max. Z-Range:                 +/- 4.55 mm
Max. Field Size (z=max):      120.594 mm
Max. X-/Y-Coordinate Value:   524288 bit


X Stretch-Factor (0 = telecentric):  0.0009878
Y Stretch-Factor (0 = telecentric):  0.000346 
Reference Point:                     (0,0,0.023) mm
(focus shifter in neutral position)

dl (max. z Control Value +32767):    4.834 mm
dl (min. z Control Value -32767):    -4.782 mm
Max. Scan Angle Mirror 1:            11.202 degrees mech.
Max. Scan Angle Mirror 2:            12.173 degrees mech.

Polynomial Coefficients for Focus Shift Control:
Focus Shift = ds (directed from z=0 opposite to z)
Control Value = A + B*ds*K_z + C*(ds*K_z)^2
16-bit:
A = 156.752
B = 12.5973
C = -2.619e-05
20-bit:
A = 2508.04
B = 12.5973
C = -1.63687e-06

Polynomial Coefficients for Distortion Correction
Image Height = f1*theta + f2*theta^2 + f3*theta^3 + f4*theta^4
Scan Angle = theta
f1 = 163.064 mm
f2 = -1.259 mm
f3 = -21.465 mm
f4 = -26.664 mm

               

