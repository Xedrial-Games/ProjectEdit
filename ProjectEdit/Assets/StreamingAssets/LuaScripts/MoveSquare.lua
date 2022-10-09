require("ProjectEdit");

export = {
	rotationSpeed = 0
};

local rotation = 0;

function Update(ts)
	-- Rotation --
	rotation = rotation + export.rotationSpeed * ts;
	transform.rotation = quat.rotate(rotation, vec3.backword);
	export.rotationSpeed = export.rotationSpeed + ts;
end
