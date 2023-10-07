extends GutTest


func before_all():
	gut.p("Testing Tests:",2)


func test_assert_gut_works():
	assert_true(true)