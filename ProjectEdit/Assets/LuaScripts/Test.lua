require("Vector3");

local position = Vector3:new(10, 20, 30);

function Start()
	Log("Start!");
	transform.position = position;
end
