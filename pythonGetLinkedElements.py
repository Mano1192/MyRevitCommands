#Copyright (c) mostafa el ayoubi
#Node-mode www.data-shapes.net 2016 elayoub.mostafa@gmail.com

import clr
clr.AddReference('RevitAPI')
from Autodesk.Revit.DB import*

clr.AddReference('ProtoGeometry')
from Autodesk.DesignScript.Geometry import *

clr.AddReference('RevitServices')
from RevitServices.Persistence import DocumentManager


doc = DocumentManager.Instance.CurrentDBDocument

#Document UI Units


UIunit = Document.GetUnits(doc).GetFormatOptions(UnitType.UT_Length).DisplayUnits

#Inputs : Points, Direction, 3D View
if isinstance(IN[0],list):
	points = [XYZ(UnitUtils.ConvertToInternalUnits(i.X,UIunit),UnitUtils.ConvertToInternalUnits(i.Y,UIunit),UnitUtils.ConvertToInternalUnits(i.Z,UIunit)) for i in IN[0]]
else:
	points = [XYZ(UnitUtils.ConvertToInternalUnits(IN[0].X,UIunit),UnitUtils.ConvertToInternalUnits(IN[0].Y,UIunit),UnitUtils.ConvertToInternalUnits(IN[0].Z,UIunit))]

direction = XYZ(IN[1].X,IN[1].Y,IN[1].Z)
view = UnwrapElement(IN[2])

ex = []
pts = []
elems = []

ri = ReferenceIntersector(view)
ri.FindReferencesInRevitLinks = True

for p in points:
	ref = ri.FindNearest(p,direction)
	if ref == None:
		pts.append(None)
		elems.append(None)
	else:
		refel = ref.GetReference()
		linkinstance = doc.GetElement(refel.ElementId)
		try:
			elem = linkinstance.GetLinkDocument().GetElement(refel.LinkedElementId)
			elems.append(elem)
			refp = ref.GetReference().GlobalPoint
			pts.append(Point.ByCoordinates(UnitUtils.ConvertFromInternalUnits(refp.X,UIunit),UnitUtils.ConvertFromInternalUnits(refp.Y,UIunit),UnitUtils.ConvertFromInternalUnits(refp.Z,UIunit)))
		except:
			if not IN[3]:
				elems.append(linkinstance)
				refp = ref.GetReference().GlobalPoint
				pts.append(Point.ByCoordinates(UnitUtils.ConvertFromInternalUnits(refp.X,UIunit),UnitUtils.ConvertFromInternalUnits(refp.Y,UIunit),UnitUtils.ConvertFromInternalUnits(refp.Z,UIunit)))
			else:
				pass


OUT = pts , elems
