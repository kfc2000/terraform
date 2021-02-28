package sg.test;

public class Test {


    public static int test() {
        /* Variable declaration */
        int f = 1;
        int i = 0;
        int sv = 1;

        while (sv != 0)
        {
            switch(sv) {
                case 1: 
                    /* Initialization */
                    i = 1;
                    sv = 2;
                    break;
                case 2: 
                    /* While Condition */
                    if (i < 10)
                        sw = 3;
                    else
                        sw = 0;
                    break;
                case 3:
                    /* While Body */
                    f = f + 1;
                    i = i + 1;
                    break;
                    
            }
        }
    }
}
