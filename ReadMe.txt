Auth
	Regularly I would make use of the built in support for authorization that comes with .Net, to not make the methods public. But for this assignment I will instead use 
	"api keys", so I can make it quicker. A real try hard method would be to also encrypt the key itself, but I feel that it is out of scope for this assignment.

Validation
	This is 'automatically' made by setting the variables as required. There could be the issue that someone sends an image that is not really an image. 
	For this test I will check the mimetype, but in production it could be beneficial to actually minimize the damage made if a "hazardous file" was to be uploaded or check the actual file.

Security
	Auth and file security I have already talked about. Because of the time constraint (and frankly that I am not getting payed for this), I will skip the more "proper ways" of security such as.

	Actually checking the real mimetype, this should be fixed on both app and api side.
	Apikeys are encrypted/hashed, not necessarily when sent and received but when stored and handled in the DB.
	There should be an endpoint for users to recreate keys, if one is found to be exposed, this should also be used by us if we find malicious usage.

Improvements
	There can be a case when a user tries to delete a quickly after someone requested a get for it, this can be fixed with threading and awaits. 
	But I feel that is a bit out of scope and opens up for serious preformance issues.