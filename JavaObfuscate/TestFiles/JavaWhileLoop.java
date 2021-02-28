package sg.test;

public class Test {


    public static int test() {
        int f = 1;

        int i = 0;

        while (i < 10)
        {
            f = f + i;
            i = i++;
        }

        return f;
    }
}
