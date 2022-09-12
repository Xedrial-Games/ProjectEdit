require("ProjectEdit");

export = {
	moveSpeed = 5,
	rotationSpeed = 5
};

local velocity = vec3.zero;
local rotation = 0;

function Start()
	local ent = CreateEntity();

	local entTransform = ent:GetComponent(Transform);
	entTransform.position = vec3.new(-5, -5, 0);

	local entSprite = ent:GetComponent(SpriteRenderer);
	entSprite.color = color.new(1, 0, 1, 1);
end

function Update(ts)
	-- Movement --
	velocity.x = Input.GetAxisRaw("Horizontal");
	velocity.y = Input.GetAxisRaw("Vertical");
	velocity = velocity:normalize():scale(export.moveSpeed * ts);
	transform.position = transform.position:add(velocity);

	-- Rotation --
	rotation = rotation + export.rotationSpeed * ts;
	transform.rotation = quat.rotate(rotation, vec3.backword);
end
