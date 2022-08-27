Vector3 =
{
	x = 0,
	y = 0,
	z = 0
};

function Vector3:new(x, y, z)
	setmetatable({}, Vector3);

	self.x = x;
	self.y = y;
	self.z = z;

	return self;
end

function Vector3:ToString()
	return string.format("%s, %s, %s", tostring(self.x), tostring(self.y), tostring(self.z));
end
