require("ProjectEdit");

export = {
	rotationSpeed = 5
};

function Update(ts)
	-- Rotation --
	transform.rotation = transform.rotation * quat.rotate(export.rotationSpeed * ts, -vec3.unit_z);
end
