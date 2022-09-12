vec3 = require("cpml.vec3");
color = require("cpml.color");
quat = require("cpml.quat");
math = require("math");

require("Entity");
require("Components");
require("GlobalFunctions");
require("Transform");
require("SpriteRenderer");

entity = Entity.new(EntityHandle);

transform = entity:GetComponent(Transform);
