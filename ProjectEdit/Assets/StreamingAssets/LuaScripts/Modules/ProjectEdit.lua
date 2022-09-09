vec3 = require("cpml.vec3");
color = require("cpml.color");
quat = require("cpml.quat");
math = require("math");

require("Entity");
require("Components");
require("GlobalFunctions");
require("Transform");

entity = Entity.new(EntityHandle);

transform = Transform.new(entity);
