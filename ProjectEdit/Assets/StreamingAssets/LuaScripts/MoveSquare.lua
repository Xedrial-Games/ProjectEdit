require("ProjectEdit");

export = {
	rotationSpeed = 5
};

local rotation = 0;

function Update(ts)
	-- Rotation --
	rotation = rotation + export.rotationSpeed * ts;
	transform.rotation = quat.rotate(rotation, vec3.backword);
end
