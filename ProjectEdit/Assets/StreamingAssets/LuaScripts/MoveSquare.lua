require("ProjectEdit");

export = {
	moveSpeed = 5,
	rotationSpeed = 5
};

local velocity = vec3.zero;
local rotation = 0;

function Start()
	local newEntity = CreateEntity();
	local newTransform = Transform.new(newEntity);
	newTransform.position = vec3.new(-5, -5, 0);
end

function Update(ts)
	rotation = rotation + export.rotationSpeed * ts;
	velocity.x = Input.GetAxisRaw("Horizontal");
	velocity.y = Input.GetAxisRaw("Vertical");
	velocity = velocity:normalize():scale(export.moveSpeed * ts);
	transform.position = transform.position:add(velocity);
	transform.rotation = quat.rotate(rotation, vec3.backword);
end
