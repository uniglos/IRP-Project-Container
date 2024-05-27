================================
Non-Euclidean Portals Package by Declan Scott (s4107812)
================================


Information about the package:
----
-This package features a prefab consisting of a pair of non-euclidean seamless portals.
-These portals are similar to those created in a video tutorial by Brackeys (accessible at https://www.youtube.com/watch?v=cuQao3hEKfs),
-However, they differ in that this package's version is programmed differently:
	> portals being coupled together and much simpler to copy and create new instances of.
	> Assets are generated during runtime for each set of portals
	> Portals have a modifyable LOD distance at which they stop updating their cameras and disable their mesh renderers
	  (this allows them to be slightly more performant, especially with many in a world)

- The package contains an additional scene with supplementary scripts to serve as an example of how the portals may be used.


---------------------------


How to use the package:
----
Simply drag and drop the PortalPair prefab asset into any scene you wish to use it in:
	All materials and rendertextures required are generated during runtime.
	This removes the need to set them up by hand each time you create a new set of portals.

Portals may be moved around freely and will teleport objects with a "Player" tag between them seamlessly.

Compatibility is built-in with the Unity Standard Assets first person controller.



---------------------------


Credits:
----
- Portal cutout shader by Brackeys : (https://www.youtube.com/watch?v=cuQao3hEKfs)
  This was used for improved rendering on the other side of portals.
  While I wanted to spend more time trying to figure this out on my own, I was not making enough progress without it.